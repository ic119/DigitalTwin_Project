using System.Collections.Generic;
using UnityEngine;

namespace JJORY.Model
{
    #region Zone Data Set
    public class ZoneData
    {
        public string zoneName { get; set; }
        public string zoneType { get; set; }
        public string zoneUnit { get; set; }
        public int zoneRow { get; set; }
        public int zoneCol { get; set; }
        public int deviceId { get; set; }
        public List<ContainerEquipmentData> containers { get; set; }
        public ZonePositionData zonePosition { get; set; }
        public int containerCount { get; set; }
    }

    public class ZonePositionData
    {
        public float x;
        public float y;
        public float z;
    }
    #endregion

    #region Container Equipment Data Set 
    /// <summary>
    /// 컨테이너(= 최상위) 장비 데이터  
    public class ContainerEquipmentData
    {
        public string containerName { get; set; }
        public string containerType { get; set; }
        public string containerUnit { get; set; }
        public int deviceId { get; set; }
        public List<RackEquipmentData> racks { get; set; }
        public int rackCount { get; set; }
        public int rackRow { get; set; }
        public int rackCol { get; set; }
    }
    #endregion

    #region Rack Equipment Data Set
    /// <summary>
    /// 랙(= 상위) 장비 데이터
    /// </summary>
    public class RackEquipmentData
    {
        public string rackName { get; set; }
        public string rackType { get; set; }
        public string rackUnit { get; set; }
        public int deviceId { get; set; }
        public int moduleCount { get; set; }
        public int moduleRow { get; set; }
        public int moduleCol { get; set; }
        public List<ModuleEquipmentData> modules { get; set; }
    }
    #endregion

    #region Pack Equipment Data Set
    /// <summary>
    /// 모듈 (= 최하위) 장비 데이터
    /// </summary>
    public class ModuleEquipmentData
    {
        public string moduleName { get; set; }
        public string moduleType { get; set; }
        public string moduleUnit { get; set; }
        public int deviceId { get; set; }
        public int cellGroupCount { get; set; }
        public int cellGroupRow { get; set; }
        public int cellGroupCol { get; set; }
        public List<CellGroupEquipmentData> cells { get; set; }
    }

    public class CellGroupEquipmentData
    {
        public string cellGroupName { get; set; }
        public string cellGroupType { get; set; }
        public string cellGroupUnit { get; set; }
        public int deviceId { get; set; }
    }
    #endregion

    #region DeviceRoot
    public class EquipmentDataRoot
    {
        public List<ZoneData> zones { get; set; }
    }
    #endregion
}
