using UnityEngine;

namespace DI
{
    public class DIExampleScene : MonoBehaviour
    {
        public void Init(DIContainer projectContainer)
        {
            var sceneContainer = new DIContainer(projectContainer);
            sceneContainer.RegisterSingleton(c => new MySceneService(c.Resolve<MyAwesomeProjectService>()));
            sceneContainer.RegisterSingleton(_ => new MyAwesomeFactory());
            sceneContainer.RegisterInstance(new MyAwesomeObject("id", 1));

            var objectsFactory = sceneContainer.Resolve<MyAwesomeFactory>();

            for (int i = 0; i < 3; i++)
            {
                var id = $"o{i}";
                var obj = objectsFactory.CreateInstance(id, i);
                Debug.Log($"Object created with factory.");
                Debug.Log($"{obj}");
            }
        }
    }
}