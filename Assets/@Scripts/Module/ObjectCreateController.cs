using JJORY.Controller;
using JJORY.Model;
using JJORY.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JJORY.Module
{
    public class ObjectCreateController : SingletonObject<ObjectCreateController>
    {
        #region Variable
        [Header("커스텀 생성 기능 관련")]
        public List<GameObject> zoneList = new List<GameObject>();

        /// <summary>
        /// 커스텀 기능에서 생성할 Zone 수량
        /// </summary>
        public int zone_Count;
        [SerializeField] private int rack_Row;
        [SerializeField] private int rack_Col = 1;
        [SerializeField] private int module_Row = 1;
        [SerializeField] private int module_Col;


        [Header("생성 게임오브젝트 관리")]
        public Dictionary<int, GameObject> instantiatedPrefab_Dictionary = new Dictionary<int, GameObject>();
        public bool isInstantiate = false;

        [Header("유저 옵션_장비 생성")]
        private float zone_Spacing = 10.0f;
        #endregion

        #region LifeCycle
        private void Awake()
        {
            AddressableController.Instance.LoadPrefabAddress<GameObject>(ModelType.ZonePrefab.ToString());
            if (EventController.Instance != null)
            {
                //EventController.Instance.OnRequestCustomGenerateDeviceSequence += InstantiateCustomDevice;
            }
        }

        private void OnDestroy()
        {
            if (EventController.Instance != null)
            {
                //EventController.Instance.OnRequestCustomGenerateDeviceSequence -= InstantiateCustomDevice;
            }
        }
        #endregion

        #region Method

        #region Custom Generate Function
        /// <summary>
        /// 커스텀 생성 기능 호출용 함수
        /// </summary>
        /// <param name="_zoneIndex"></param>
        /// <param name="_containerCount"></param>
        /// <param name="_rackCount"></param>
        /// <param name="_packCount"></param>
        private void InstantiateCustomDevice(int _zoneIndex, int _containerCount, int _rackCount, int _packCount)
        {
            InstantiateCustomContainerDevice(_zoneIndex, _containerCount, _rackCount, _packCount);
        }

        /// <summary>
        /// 커스텀 생성 기능_BESS Container 오브젝트 생성 함수
        /// </summary>
        /// <param name="_zoneIndex"></param>
        /// <param name="_containerCount"></param>
        /// <param name="_rackCount"></param>
        /// <param name="_packCount"></param>
        public void InstantiateCustomContainerDevice(int _zoneIndex, int _containerCount, int _rackCount, int _packCount)
        {
            AsyncOperationHandle custom_Handler;
            string key = ModelType.ContainerPrefab.ToString();

            GameObject zone = zoneList[_zoneIndex];
            EquipmentProperty zoneProperty = zone.GetComponent<EquipmentProperty>();

            zoneProperty.ZoneProperty.SetContainerCount(_containerCount);

            // custom_Handler 체크
            if (AddressableController.Instance.GetHandler(key, out custom_Handler))
            {
                GameObject prefab = custom_Handler.Result as GameObject;
                for (int i = 0; i < _containerCount; i++)
                {
                    GameObject container = AddressableController.Instance.InstantiatePrefab(prefab);
                    container.name = "ESS_Container_" + i;

                    container.transform.SetParent(zoneProperty.containersRoot, false);

                    EquipmentProperty containerProperty = container.GetComponent<EquipmentProperty>();

                    //containerProperty.ObjectNameSetting(key, EquipmentUnit.Container);

                    InstantiateCustomRackDevice(container, _rackCount, _packCount);
                }
            }
            else
            {
                Utils.CreateLogMessage<ObjectCreateController>($"{key}에 대한 Handler 없음");
            }
        }

        /// <summary>
        /// 커스텀 생성 기능_BESS Rack 오브젝트 생성 함수
        /// </summary>
        /// <param name="_container"></param>
        /// <param name="_rackCount"></param>
        /// <param name="_packCount"></param>
        public void InstantiateCustomRackDevice(GameObject _container, int _rackCount, int _packCount)
        {
            EquipmentProperty containerProperty = _container.GetComponent<EquipmentProperty>();
            containerProperty.ContainerProperty.RackRow = _rackCount;
            if (_rackCount == 0)
            {
                containerProperty.rackEx.SetActive(false);
            }
            else
            {
                containerProperty.ContainerProperty.SetRackCount(_rackCount);

                AsyncOperationHandle custom_Handler;
                string key = ModelType.RackPrefab.ToString();

                if (AddressableController.Instance.GetHandler(key, out custom_Handler))
                {
                    GameObject prefab = custom_Handler.Result as GameObject;

                    for (int y = 0; y < containerProperty.ContainerProperty.RackCol; y++)
                    {
                        for (int x = 0; x < containerProperty.ContainerProperty.RackRow; x++)
                        {
                            int idx = y * containerProperty.ContainerProperty.RackRow + x;

                            if (idx >= _rackCount)
                            {
                                break;
                            }

                            GameObject rack = AddressableController.Instance.InstantiatePrefab(prefab);
                            rack.name = "ESS_Container_Rack_" + idx;
                            rack.transform.SetParent(containerProperty.racksRoot, false);

                            if (idx == 0)
                            {
                                rack.transform.position = containerProperty.rackEx.transform.position;
                                containerProperty.rackEx.SetActive(false);
                            }

                            rack.transform.position = new Vector3(containerProperty.rackEx.transform.position.x - (containerProperty.rackEx.transform.localScale.x * x),
                                                                  containerProperty.rackEx.transform.position.y,
                                                                  containerProperty.rackEx.transform.position.z);

                            EquipmentProperty rackProperty = rack.GetComponent<EquipmentProperty>();
                            //rackProperty.ObjectNameSetting(key, EquipmentUnit.Rack);

                            InstantiateCustomModuleDevice(rack, _packCount);
                        }
                    }
                }
                else
                {
                    Utils.CreateLogMessage<ObjectCreateController>($"{key}에 대한 Handler 없음");
                }
            }
        }

        /// <summary>
        /// 커스텀 생성 기능_BESS Pack 오브젝트 생성 함수
        /// </summary>
        /// <param name="_rack"></param>
        /// <param name="_packCount"></param>
        public void InstantiateCustomModuleDevice(GameObject _rack, int _moduleCount)
        {
            EquipmentProperty rackProperty = _rack.GetComponent<EquipmentProperty>();
            rackProperty.RackProperty.ModuleCol = _moduleCount;

            if (_moduleCount == 0)
            {
                return;
            }
            else
            {
                rackProperty.RackProperty.SetModuleCount(_moduleCount);

                AsyncOperationHandle custom_Handler;
                string key = ModelType.ModulePrefab.ToString();

                if (AddressableController.Instance.GetHandler(key, out custom_Handler))
                {
                    GameObject prefab = custom_Handler.Result as GameObject;

                    for (int x = 0; x < rackProperty.RackProperty.ModuleRow; x++)
                    {
                        for (int y = 0; y < rackProperty.RackProperty.ModuleCol; y++)
                        {
                            int idx = x * rackProperty.RackProperty.ModuleCol + y;

                            if (idx == _moduleCount)
                            {
                                break;
                            }

                            GameObject module = AddressableController.Instance.InstantiatePrefab(prefab);
                            module.name = "ESS_Container_Rack_Module_" + idx;
                            module.transform.SetParent(rackProperty.modulesRoot, false);

                            if (idx == 0)
                            {
                                module.transform.position = rackProperty.moduleEx.transform.position;
                                rackProperty.moduleEx.SetActive(false);
                            }

                            module.transform.position = new Vector3(rackProperty.moduleEx.transform.position.x,
                                                                    rackProperty.moduleEx.transform.position.y + (rackProperty.moduleEx.transform.lossyScale.y * y),
                                                                    rackProperty.moduleEx.transform.position.z);

                            EquipmentProperty packProperty = module.GetComponent<EquipmentProperty>();
                            //packProperty.ObjectNameSetting(key, EquipmentUnit.Module);
                        }
                    }
                }
            }
        }
        #endregion

        #region Serial Number Search Instantiate
        /// <summary>
        /// BESS 시리얼 넘버 조회 후 생성 처리 로직
        /// </summary>
        /// <param name="_data"></param>
        /// <returns></returns>
        public IEnumerator AsyncSerialInstantiate(ZoneData _data)
        {
            yield return null;
            if (JsonDataController.Instance.jsonData_List.Count > 0)
            {
                yield return InstantiateZonePrefab(_data);
                isInstantiate = true;
            }
        }
        #endregion

        #region Client Instantiate
        /// <summary>
        /// JsonData에 의한 비동기 오브젝트 생성
        /// </summary>
        public IEnumerator AsyncInstantiate()
        {
            yield return null;
            if (JsonDataController.Instance.jsonData_List.Count > 0)
            {
                foreach (ZoneData data in JsonDataController.Instance.jsonData_List)
                {
                    yield return InstantiateZonePrefab(data);
                }
                isInstantiate = true;
            }
        }

        /// <summary>
        /// JsonData에 의한 비동기 구역 오브젝트 생성
        /// </summary>
        /// <param name="_zoneData"></param>
        /// <returns></returns>
        private IEnumerator InstantiateZonePrefab(ZoneData _zoneData)
        {
            AsyncOperationHandle handler;

            while (!AddressableController.Instance.GetHandler(_zoneData.zoneType, out handler))
            {
                yield return null;
            }

            while (!handler.IsDone)
            {
                yield return null;
            }

            GameObject prefab = handler.Result as GameObject;
            GameObject zone = AddressableController.Instance.InstantiatePrefab(prefab);

            if (zone == null)
            {
                yield return null;
            }

            if (zone.GetComponent<EquipmentProperty>() == null)
            {
                zone.AddComponent<EquipmentProperty>();
            }

            EquipmentProperty zoneProperty = zone.GetComponent<EquipmentProperty>();

            //zoneProperty.ZonePropertyInit(_zoneData);
            zone.name = zoneProperty.equipmentName;

            //if (zone.GetComponent<ZoneInfoView>() != null)
            //{
            //    zone.GetComponent<ZoneInfoView>().ZoneTitleSetting(zone.name);
            //}

            zone.transform.SetParent(transform);
            zone.transform.position = new Vector3(_zoneData.zonePosition.x,
                                                  _zoneData.zonePosition.y,
                                                  _zoneData.zonePosition.z);
            zoneList.Add(zone);

            if (instantiatedPrefab_Dictionary.ContainsKey(_zoneData.deviceId) == false)
            {
                instantiatedPrefab_Dictionary.Add(_zoneData.deviceId, zone);
            }

            if (_zoneData.containers.Count > 0)
            {
                yield return InstantiateContainerPrefab(zone, _zoneData);
                zoneProperty.ZoneProperty.SetHasBESS(true);
            }
            else
            {
                zoneProperty.ZoneProperty.SetHasBESS(false);
            }
        }

        /// <summary>
        /// JsonData에 의한 비동기 BESS Container 오브젝트 생성
        /// </summary>
        /// <param name="_zone"></param>
        /// <param name="_zoneData"></param>
        /// <returns></returns>
        private IEnumerator InstantiateContainerPrefab(GameObject _zone, ZoneData _zoneData)
        {
            AsyncOperationHandle handler;
            EquipmentProperty zoneProperty = _zone.GetComponent<EquipmentProperty>();

            ContainerEquipmentData containerData = null;
            foreach (var data in _zoneData.containers)
            {
                containerData = data;
            }

            while (!AddressableController.Instance.GetHandler(containerData.containerType, out handler))
            {
                yield return null;
            }

            while (!handler.IsDone)
            {
                yield return null;
            }

            GameObject prefab = handler.Result as GameObject;
            GameObject container = AddressableController.Instance.InstantiatePrefab(prefab);

            if (container == null)
            {
                yield return null;
            }

            if (container.GetComponent<EquipmentProperty>() == null)
            {
                container.AddComponent<EquipmentProperty>();
            }

            EquipmentProperty containerProperty = container.GetComponent<EquipmentProperty>();

            containerProperty.ContainerPropertyInit(containerData);
            container.name = containerProperty.equipmentName;

            container.transform.SetParent(zoneProperty.containersRoot, false);

            if (instantiatedPrefab_Dictionary.ContainsKey(containerData.deviceId) == false)
            {
                instantiatedPrefab_Dictionary.Add(containerData.deviceId, container);
                Utils.CreateLogMessage<ObjectCreateController>($"{containerData.containerName}_{containerData.deviceId} 생성 완료");
            }

            if (containerData.rackCount > 0)
            {
                yield return InstantiateRackPrefab(container, containerData);
            }
        }

        /// <summary>
        /// JsonData에 의한 비동기 Rack 오브젝트 생성
        /// </summary>
        /// <param name="_container"></param>
        /// <param name="_containerData"></param>
        /// <returns></returns>
        private IEnumerator InstantiateRackPrefab(GameObject _container, ContainerEquipmentData _containerData)
        {
            EquipmentProperty containerProperty = _container.GetComponent<EquipmentProperty>();
            AsyncOperationHandle handler;

            for (int y = 0; y < _containerData.rackCol; y++)
            {
                for (int x = 0; x < _containerData.rackRow; x++)
                {
                    int idx = y * _containerData.rackRow + x;
                    if (idx >= _containerData.rackCount)
                    {
                        yield break;
                    }

                    RackEquipmentData rackData = _containerData.racks[idx];

                    while (!AddressableController.Instance.GetHandler(rackData.rackType, out handler))
                    {
                        yield return null;
                    }

                    // 로딩 완료될 때까지 대기
                    while (!handler.IsDone)
                    {
                        yield return null;
                    }

                    GameObject prefab = handler.Result as GameObject;
                    GameObject rack = AddressableController.Instance.InstantiatePrefab(prefab);
                    rack.transform.SetParent(containerProperty.racksRoot, false);

                    if (idx == 0)
                    {
                        rack.transform.position = containerProperty.rackEx.transform.position;
                        containerProperty.rackEx.SetActive(false);
                    }
                    else
                    {
                        rack.transform.position = new Vector3(containerProperty.rackEx.transform.position.x - (containerProperty.rackEx.transform.localScale.x * x),
                                                              containerProperty.rackEx.transform.position.y,
                                                              containerProperty.rackEx.transform.position.z);
                    }

                    EquipmentProperty rackProperty = rack.GetComponent<EquipmentProperty>();

                    rackProperty.RackPropertyInit(rackData);
                    rackProperty.parentContainer = _container;
                    rack.name = rackData.rackName;

                    if (instantiatedPrefab_Dictionary.ContainsKey(rackData.deviceId) == false)
                    {
                        instantiatedPrefab_Dictionary.Add(rackData.deviceId, rack);
                    }

                    if (rackData.moduleCount > 0)
                    {
                        yield return InstantiateModulePrafab(rack, rackData);
                    }
                }
            }
            containerProperty.racksRoot.gameObject.SetActive(false);
        }

        /// <summary>
        /// JsonData에 의한 비동기 Pack 오브젝트 생성
        /// </summary>
        /// <param name="_rack"></param>
        /// <param name="_rackData"></param>
        /// <param name="_rackData"></param>
        /// <returns></returns>
        private IEnumerator InstantiateModulePrafab(GameObject _rack, RackEquipmentData _rackData)
        {
            EquipmentProperty rackProperty = _rack.GetComponent<EquipmentProperty>();

            AsyncOperationHandle handler;

            for (int x = 0; x < _rackData.moduleRow; x++)
            {
                for (int y = 0; y < _rackData.moduleCol; y++)
                {
                    int idx = x * _rackData.moduleCol + y;

                    if (idx >= _rackData.moduleCount)
                    {
                        yield break;
                    }

                    ModuleEquipmentData moduleData = _rackData.modules[idx];

                    while (!AddressableController.Instance.GetHandler(moduleData.moduleType, out handler))
                    {
                        yield return null;
                    }

                    // 로딩 완료될 때까지 대기
                    while (!handler.IsDone)
                    {
                        yield return null;
                    }

                    GameObject prefab = handler.Result as GameObject;
                    GameObject module = AddressableController.Instance.InstantiatePrefab(prefab);
                    module.transform.SetParent(rackProperty.modulesRoot, false);

                    EquipmentProperty moduleProperty = module.GetComponent<EquipmentProperty>();
                    moduleProperty.ModulePropertyInit(moduleData);

                    moduleProperty.parentContainer = rackProperty.parentContainer;
                    module.name = moduleData.moduleName;

                    if (idx == 0)
                    {
                        module.transform.position = rackProperty.moduleEx.transform.position;
                        rackProperty.moduleEx.SetActive(false);
                    }

                    module.transform.position = new Vector3(rackProperty.moduleEx.transform.position.x,
                                                            rackProperty.moduleEx.transform.position.y + (rackProperty.moduleEx.transform.lossyScale.y + 0.4f * idx),
                                                            rackProperty.moduleEx.transform.position.z);

                    if (instantiatedPrefab_Dictionary.ContainsKey(moduleData.deviceId) == false)
                    {
                        instantiatedPrefab_Dictionary.Add(moduleData.deviceId, module);
                    }

                    if (moduleData.cellGroupCount > 0)
                    {
                        yield return InstantiateCellGroupPrafab(module, moduleData);
                    }
                }
            }
            rackProperty.modulesRoot.gameObject.SetActive(false);
        }

        private IEnumerator InstantiateCellGroupPrafab(GameObject _module, ModuleEquipmentData _moduleData)
        {
            EquipmentProperty moduleProperty = _module.GetComponent<EquipmentProperty>();

            AsyncOperationHandle handler;

            for (int x = 0; x < _moduleData.cellGroupRow; x++)
            {
                for (int y = 0; y < _moduleData.cellGroupCol; y++)
                {
                    int idx = (x * _moduleData.cellGroupCol) + y;

                    if (idx >= _moduleData.cellGroupCount)
                    {
                        yield break;

                    }
                    CellGroupEquipmentData cellGroupData = _moduleData.cells[idx];

                    while (!AddressableController.Instance.GetHandler(cellGroupData.cellGroupType, out handler))
                    {
                        yield return null;
                    }

                    // 로딩 완료될 때까지 대기
                    while (!handler.IsDone)
                    {
                        yield return null;
                    }

                    GameObject prefab = handler.Result as GameObject;
                    GameObject cellGroup = AddressableController.Instance.InstantiatePrefab(prefab);
                    cellGroup.transform.SetParent(moduleProperty.cellsRoot, false);

                    EquipmentProperty cellGroupProperty = cellGroup.GetComponent<EquipmentProperty>();
                    cellGroupProperty.CellGroupPropertyInit(cellGroupData);

                    cellGroupProperty.parentContainer = moduleProperty.parentContainer;
                    cellGroup.name = cellGroupData.cellGroupName;

                    if (idx == 0)
                    {
                        cellGroup.transform.position = moduleProperty.cellEx.transform.position;
                        moduleProperty.cellEx.SetActive(false);
                    }

                    cellGroup.transform.position = new Vector3(moduleProperty.cellEx.transform.position.x - (0.9f * x),
                                                               moduleProperty.cellEx.transform.position.y,
                                                               moduleProperty.cellEx.transform.position.z + (moduleProperty.cellEx.transform.lossyScale.z / 2.5f * y));

                    if (instantiatedPrefab_Dictionary.ContainsKey(cellGroupData.deviceId) == false)
                    {
                        instantiatedPrefab_Dictionary.Add(cellGroupData.deviceId, cellGroup);
                    }
                }
            }
            moduleProperty.cellsRoot.gameObject.SetActive(false);
        }
        #endregion

        /// <summary>
        /// 유저 옵션 기능 - Zone 영역 생성
        /// </summary>
        /// <param name="_count"></param>   
        public void CustomGenerateGridZone(int _count)
        {
            if (_count <= 0)
            {
                return;
            }

            if (AddressableController.Instance == null || ObjectCreateController.Instance == null)
            {
                return;
            }

            int start_Index = zoneList.Count;
            int end_Index = start_Index + _count;

            int grid_Size = Mathf.CeilToInt(Mathf.Sqrt(end_Index));

            int cur_Index = 0;

            for (int x = 0; x < grid_Size; x++)
            {
                for (int z = 0; z < grid_Size; z++)
                {
                    if (cur_Index >= end_Index)
                    {
                        break;
                    }

                    if (cur_Index >= zoneList.Count)
                    {
                        GameObject zone = AddressableController.Instance.InstantiatePrefabHelper<GameObject>(ModelType.ZonePrefab.ToString());

                        zone.transform.SetParent(ObjectCreateController.Instance.transform, false);
                        zone.transform.position = new Vector3(x * zone_Spacing, 0, z * zone_Spacing);
                        zone.name = $"Zone {cur_Index + 1}";
                        zone.transform.localScale = new Vector3(1f, 1f, 1f);

                        zoneList.Add(zone);
                        //ZoneInfoView view = zone.GetComponent<ZoneInfoView>();

                        //if (view != null)
                        //{
                        //    view.ZoneTitleSetting(zone.name);
                        //}
                        //else
                        //{
                        //    Utils.CreateLogMessage<ObjectCreateController>("ZoneInfoView is null");
                        //}
                    }
                    cur_Index++;
                }
            }
        }

        /// <summary>
        /// Row & Col 값 입력받아 처리 버전
        /// </summary>
        /// <param name="_row"></param>
        /// <param name="_col"></param>
        public void CustomGenerateGridZone(int _row, int _col)
        {
            if (_row <= 0)
            {
                _row = 1;
            }

            if (_col <= 0)
            {
                _col = 1;
            }

            if (AddressableController.Instance == null || ObjectCreateController.Instance == null)
            {
                return;
            }

            int count = _row * _col;

            int startIndex = zoneList.Count;
            int endIndex = startIndex + count;

            int curIndex = 0;

            for (int x = 0; x < _row; x++)
            {
                for (int z = 0; z < _col; z++)
                {
                    if (curIndex >= count)
                    {
                        return;
                    }

                    if (curIndex >= zoneList.Count)
                    {
                        GameObject zone = AddressableController.Instance.InstantiatePrefabHelper<GameObject>(ModelType.ZonePrefab.ToString());

                        zone.transform.SetParent(ObjectCreateController.Instance.transform, false);
                        zone.transform.position = new Vector3(x * zone_Spacing, 0, z * zone_Spacing);
                        zone.name = $"Zone {curIndex + 1}";
                        zone.transform.localScale = new Vector3(1f, 1f, 1f);
                        zoneList.Add(zone);
                        //ZoneInfoView view = zone.GetComponent<ZoneInfoView>();

                        //if (view != null)
                        //{
                        //    view.ZoneTitleSetting(zone.name);
                        //}
                        //else
                        //{
                        //    Utils.CreateLogMessage<ObjectCreateController>("ZoneInfoView is null");
                        //}
                    }

                    curIndex++;
                }
            }
        }
        #endregion
    }
}
