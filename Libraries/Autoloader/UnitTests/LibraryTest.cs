using Braxnet;
using Sandbox;

[TestClass]
public partial class LibraryTests
{
	[TestMethod]
	public void SceneTest()
	{
		var scene = new Scene();
		using ( scene.Push() )
		{
			// var go = new GameObject();
			Assert.AreEqual( 1, scene.Directory.GameObjectCount );
			Assert.IsTrue( scene.Directory.FindByName( "LibraryTestComponent" ) != null );
		}
	}

}

[Autoload]
public class LibraryTestComponent : Component
{
	
}
