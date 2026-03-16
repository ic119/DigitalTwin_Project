using JJORY.Model;
using JJORY.Util;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


namespace JJORY.Controller
{
    public class JsonDataController : SingletonObject<JsonDataController>
    {
        #region Variable
        [Header("Local Test Json Key")]
        [SerializeField] private string key;

        [Header("Parse Data List")]
        public List<ZoneData> jsonData_List = new List<ZoneData>();

        [Header("React Parsing State")]
        public bool isReactParsing = false;
        public bool isReactParsed = false;

        /// <summary>SendUnityReady 1회만 호출용 (JsonDataLoader 인스턴스 생성 후 React에 알림)</summary>
        private static bool _hasSentUnityReady;

        private long _currentParseVersion = 0;
        /// <summary>React 첫 수신 시에만 AddKeyHashSet/LoadPrefabAddressFromHashSet 호출. 재생성 시에는 스킵.</summary>
        private bool _hasCompletedFirstReactLoad = false;
        #endregion

        #region LifeCycle
        private void Start()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (_hasSentUnityReady)
                return;
            _hasSentUnityReady = true;
         
            try
            {
                MonitoringBridge.SendUnityReady();
            }
            catch (Exception e)
            {
                Utils.CreateLogMessage<JsonDataController>($"SendUnity Ready failed : {e.Message}");
            }
#endif
        }
        #endregion

        #region Method
        #endregion
    }
}
