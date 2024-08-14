using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Braxnet;

/// <summary>
///  Creates objects with components that have the Autoload attribute. Will stay between scenes.
/// </summary>
public sealed class Autoloader : GameObjectSystem
{
	
	private readonly Dictionary<TypeDescription, Component> _components = new();
	
	public T GetComponent<T>() where T : Component
	{
		if ( _components.TryGetValue( TypeLibrary.GetType<T>(), out var comp ) )
		{
			return (T)comp;
		}

		return null;
	}

	private bool HasComponent( TypeDescription type )
	{
		return Scene.GetAllComponents( type.GetType() ).Any();
	}

	public Autoloader( Scene scene ) : base( scene )
	{
		Log.Trace( $"Autoloader initialized on scene {scene.Name}" );

		var autoloadTypes = TypeLibrary.GetTypesWithAttribute<AutoloadAttribute>().ToList();

		if ( !autoloadTypes.Any() )
		{
			Log.Trace( "No types to autoload" );
			return;
		}

		foreach ( var type in autoloadTypes )
		{
			if ( !HasComponent( type.Type ) )
			{
				if ( type.Attribute.Prefab != null )
				{
					var obj = SceneUtility.GetPrefabScene( ResourceLibrary.Get<PrefabFile>( type.Attribute.Prefab ) ).Clone();
					_components[type.Type] = obj.Components.Get( type.Type.GetType() );
					obj.Name = type.Type.Name;
					obj.Flags |= GameObjectFlags.DontDestroyOnLoad;
					Log.Trace( $"Autoloading prefab {type.Type.Name} -> {obj}" );
				}
				else
				{
					var obj = Scene.CreateObject();
					_components[type.Type] = obj.Components.Create( type.Type );
					obj.Name = type.Type.Name;
					obj.Flags |= GameObjectFlags.DontDestroyOnLoad;
					Log.Trace( $"Autoloading {type.Type.Name} -> {obj}" );
				}
			}
		}
	}
}

/// <summary>
///  Components with this attribute will be created by the Autoloader on scene start.
/// </summary>
[AttributeUsage( AttributeTargets.Class )]
public sealed class AutoloadAttribute : Attribute
{
	public string Prefab { get; set; }
	public AutoloadAttribute()
	{
	}
	
	public AutoloadAttribute( string prefab )
	{
		Prefab = prefab;
	}
}
