using JJORY.Module;
using JJORY.Scene.Dummy;
using JJORY.Util;
using System.Collections;
using UnityEngine;

namespace JJORY.Controller
{
    public class BootstrapController : MonoBehaviour
    {
        #region Class Variable
        class SceneLoadManager : Sequence
        {
            public IEnumerator Execute()
            {
                bool isFlag = false;
                SceneLoadController.Instance.Init(() =>
                {
                    isFlag = true;
                });
                while (!isFlag)
                {
                    yield return null;
                }
                Utils.CreateLogMessage<BootstrapController>("1. SceneLoadManager LoadComplete!");
            }
        }

        class SceneChangeManager : Sequence 
        {
            public IEnumerator Execute()
            {
                yield return null;
                Utils.CreateLogMessage<BootstrapController>("2. SceneChangeManager LoadComplete!");
                SceneLoadController.Instance.LoadSceneByTags("main");
            }
        }

        class AddressableManager : Sequence
        {
            public IEnumerator Execute()
            {
                if (AddressableController.Instance != null)
                {
                    //AddressableController.Instance.Init();
                }

                yield return null;

                Utils.CreateLogMessage<BootstrapController>("3. AddressableManage LoadComplete!");
            }
        }

        class JsonDataManager : Sequence
        {
            public IEnumerator Execute()
            {
#if UNITY_EDITOR
                //JsonDataCon.Instance.Init();
#endif
                yield return null;

                Utils.CreateLogMessage<BootstrapController>("4. JsonDataLoadManage LoadComplete!");
            }
        }

        class ReactCommunicationManager : Sequence
        {
            public IEnumerator Execute()
            {
                // React 브리지 매니저를 미리 생성(DontDestroyOnLoad)해 통신 라인을 준비
                //MonitoringBridge monitoringBridge = MonitoringBridge.Instance;

                Utils.CreateLogMessage<BootstrapController>("5. ReactWebGLManage LoadComplete!");
                yield return null;
            }
        }
        #endregion

        #region Variable
        #endregion

        #region LifeCycle
        private void Start()
        {
            
        }
        #endregion

        #region Method
        private void ModuleSetting()
        {
            SceneLoadManager sceneLoadManager = new SceneLoadManager();
            SceneChangeManager sceneChangeManager = new SceneChangeManager();
            AddressableManager addressableManager = new AddressableManager();
            JsonDataManager jsonDataManager = new JsonDataManager();
            ReactCommunicationManager reactCommunicationManager = new ReactCommunicationManager();


            SequenceActionUtils.Instance.Enqueue(sceneLoadManager);
            SequenceActionUtils.Instance.Enqueue(sceneChangeManager);
            SequenceActionUtils.Instance.Enqueue(addressableManager);
            SequenceActionUtils.Instance.Enqueue(jsonDataManager);
            SequenceActionUtils.Instance.Enqueue(reactCommunicationManager);

            SequenceActionUtils.Instance.DoSequenceAction();
        }
        #endregion
    }
}
