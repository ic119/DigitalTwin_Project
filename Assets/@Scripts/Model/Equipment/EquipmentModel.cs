using System.Collections.Generic;
using UnityEngine;

namespace JJORY.Model
{
    #region Zone Data Set
    public class ZoneData
    {
        public string serial_No { get; set; }
        public string zone_Name { get; set; }
        public string zone_Type { get; set; }
        public string zone_Unit { get; set; }
        public int zone_Row { get; set; }
        public int zone_Col { get; set; }
        public string device_Id { get; set; }
        public List<ContainerEquipmentData> containers { get; set; }
        public ZonePositionData zone_Position { get; set; }
        public int container_Count { get; set; }
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
        public string container_Name { get; set; }
        public string container_Type { get; set; }
        public string container_Unit { get; set; }
        public string device_Id { get; set; }
        public List<RackEquipmentData> racks { get; set; }
        public int rack_Count { get; set; }
        public int rack_row { get; set; }
        public int rack_col { get; set; }
    }
    #endregion

    #region Rack Equipment Data Set
    /// <summary>
    /// 랙(= 상위) 장비 데이터
    /// </summary>
    public class RackEquipmentData
    {
        public string rack_Name { get; set; }
        public string rack_Type { get; set; }
        public string rack_Unit { get; set; }
        public string device_Id { get; set; }
        public int module_Count { get; set; }
        public int module_row { get; set; }
        public int module_col { get; set; }
        public List<ModuleEquipmentData> modules { get; set; }
    }
    #endregion

    #region Pack Equipment Data Set
    /// <summary>
    /// 모듈 (= 최하위) 장비 데이터
    /// </summary>
    public class ModuleEquipmentData
    {
        public string module_Name { get; set; }
        public string module_Type { get; set; }
        public string module_Unit { get; set; }
        public string device_Id { get; set; }
        public int cellGroup_Count { get; set; }
        public int cellGroup_row { get; set; }
        public int cellGroup_col { get; set; }
        public List<CellGroupEquipmentData> cells { get; set; }
    }

    public class CellGroupEquipmentData
    {
        public string cellGroup_Name { get; set; }
        public string cellGroup_Type { get; set; }
        public string cellGroup_Unit { get; set; }
        public string device_Id { get; set; }
    }
    #endregion

    #region Event Data Set
    public class EventEquipmentData
    {
        public string device_Id { get; set; }
        public string device_Name { get; set; }
        public string device_Unit { get; set; }
    }
    #endregion

    #region DeviceRoot
    public class EquipmentDataRoot
    {
        public List<ZoneData> zones { get; set; }
    }
    #endregion
}
