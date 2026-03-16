using System;
using System.Collections.Generic;
using UnityEngine;


namespace JJORY.Model
{
    [Serializable]
    public class ZoneProperty
    {
        [SerializeField] private int zoneRow;
        [SerializeField] private int zoneCol;
        [SerializeField] private int containerCount;
        [SerializeField] private bool hasBESS;
        public int ZoneRow => zoneRow;
        public int ZoneCol => zoneCol;
        public int ContainerCount => containerCount;
        public bool HasBESS => hasBESS;

        /// <summary>
        /// 생성된 구역의 row,col 설정
        /// </summary>
        /// <param name="_row"></param>
        /// <param name="_col"></param>
        public void SetZoneProperty(int _row, int _col)
        {
            zoneRow = _row;
            zoneCol = _col;
        }

        /// <summary>
        /// ZoneProperty의 containerCount 값 세팅
        /// </summary>
        /// <param name="_value"></param>
        public void SetContainerCount(int _value)
        {
            containerCount = _value;
        }

        public void SetHasBESS(bool _value)
        {
            hasBESS = _value;
        }
    }

    [Serializable]
    public class ContainerProperty
    {
        [SerializeField] private int rackCount;
        [SerializeField] private int rackRow;
        [SerializeField] private int rackCol = 1;

        public int RackCount => rackCount;
        public int RackRow
        {
            get => rackRow;
            set => rackRow = value;
        }
        public int RackCol => rackCol;

        /// <summary>
        /// ContainerProperty의 rackCount 값 세팅
        /// </summary>
        /// <param name="_value"></param>
        public void SetRackCount(int _value)
        {
            rackCount = _value;
        }

        /// <summary>
        /// ContainerProperty의 rackRow 값 세팅
        /// </summary>
        /// <param name="_value"></param>
        public void SetRackRow(int _value)
        {
            rackRow = _value;
        }
    }

    [Serializable]
    public class RackProperty
    {
        [SerializeField] private int moduleCount;
        [SerializeField] private int moduleRow = 1;
        [SerializeField] private int moduleCol;
        public int ModuleCount => moduleCount;
        public int ModuleRow => moduleRow;
        public int ModuleCol
        {
            get => moduleCol;
            set => moduleCol = value;
        }

        /// <summary>
        /// RackProperty의 moduleCount 값 세팅
        /// </summary>
        /// <param name="_value"></param>
        public void SetModuleCount(int _value)
        {
            moduleCount = _value;
        }

        /// <summary>
        /// RackProperty의 moduleRow 값 세팅
        /// </summary>
        /// <param name="_value"></param>
        public void SetModuleCol(int _value)
        {
            moduleCol = _value;
        }
    }

    [Serializable]
    public class ModuleProperty
    {
        [SerializeField] private int cellGroupCount;
        [SerializeField] private int cellGroupRow;
        [SerializeField] private int cellGroupCol;
        [SerializeField] private int cellGroupMaxCol;

        public int CellGroupCount => cellGroupCount;
        public int CellGroupRow => cellGroupRow;
        public int CellGroupMaxCol => cellGroupMaxCol;

        public int CellGroupCol
        {
            get => cellGroupCol;
            set => cellGroupCol = value;
        }

        /// <summary>
        /// ModuleProperty의 cellGroupCount 값 세팅
        /// </summary>
        /// <param name="_value"></param>
        public void SetCellGroupCount(int _value)
        {
            cellGroupCount = _value;
        }

        /// <summary>
        /// ModuleProperty의 cellGroupRow 값 세팅
        /// </summary>
        /// <param name="_value"></param>
        public void SetCellGroupCol(int _value)
        {
            cellGroupCol = _value;
        }
    }

    public class EquipmentProperty : MonoBehaviour
    {
        #region Variable
        [Header("장비 공통 데이터")]
        public string equipment_Name { get; private set; }
        public string equipment_Type { get; private set; }
        public EquipmentUnit equipment_Unit { get; private set; }
        public string equipment_id { get; private set; }
        public Vector3 equipment_Position { get; private set; }


        [Header("BESS 내부 장비오브젝트 Root")]
        public Transform containersRoot;
        public Transform racksRoot;
        public Transform modulesRoot;
        public Transform cellsRoot;
        public GameObject rackEx;
        public GameObject moduleEx;
        public GameObject cellEx;
        public GameObject moduleCover;

        [Header("하위 계층 장비 관리 리스트")]
        public List<GameObject> rackList = new List<GameObject>();
        public List<GameObject> moduleList = new List<GameObject>();

        [Header("Object Material Change")]
        public Material containerBase_Mt;
        public Material containerTranslucent_Mt;
        public GameObject containerBody;
        public List<MeshRenderer> containerDoors_Mr = new List<MeshRenderer>();
        public MeshRenderer containerBody_Mr;

        [Header("Container Door Object")]
        public List<GameObject> containerLeftDoor_List = new List<GameObject>();
        public List<GameObject> containetRightDoor_List = new List<GameObject>();

        [Header("BESS Property Variable")]
        [SerializeField] private ZoneProperty zoneProperty = new ZoneProperty();
        [SerializeField] private ContainerProperty containerProperty = new ContainerProperty();
        [SerializeField] private RackProperty rackProperty = new RackProperty();
        [SerializeField] private ModuleProperty moduleProperty = new ModuleProperty();

        [Header("Rack 관련 변수")]
        public GameObject rackBody;

        [Header("최상위 및 상위 오브젝트 변수")]
        public GameObject zoneObject;
        public GameObject rootObject;
        public GameObject parentObject;

        [Header("콜라이더 변수")]
        public BoxCollider curObjectCollider;

        public ZoneProperty ZoneProperty => zoneProperty;
        public ContainerProperty ContainerProperty => containerProperty;
        public RackProperty RackProperty => rackProperty;
        public ModuleProperty ModuleProperty => moduleProperty;

        public bool isEvent { get; set; } = false;
        public bool isClicked { get; set; } = false;
        #endregion

        #region Method
        /// <summary>
        /// Container 오브젝트에 대한 Property 값 저장
        /// </summary>
        /// <param name="_data"></param>z
        public void ContainerPropertyInit(ContainerEquipmentData _data)
        {
            equipment_id = _data.device_Id;
            equipment_Name = _data.container_Name;
            equipment_Type = _data.container_Type;
            if (_data.container_Unit.Equals(EquipmentUnit.Container.ToString()))
            {
                equipment_Unit = EquipmentUnit.Container;
            }
            containerProperty.SetRackCount(_data.rack_Count);
            containerProperty.SetRackRow(_data.rack_row);
        }

        /// <summary>
        /// Rack 오브젝트에 대한 Property 값 저장
        /// </summary>
        /// <param name="_data"></param>
        public void RackPropertyInit(RackEquipmentData _data)
        {
            equipment_id = _data.device_Id;
            equipment_Name = _data.rack_Name;
            equipment_Type = _data.rack_Type;

            if (_data.rack_Unit.Equals(EquipmentUnit.Rack.ToString()))
            {
                equipment_Unit = EquipmentUnit.Rack;
            }

            rackProperty.SetModuleCount(_data.module_Count);
            rackProperty.SetModuleCol(_data.module_col);
        }

        /// <summary>
        /// 생성된 Rack의 Oringnal Position값 저장
        /// </summary>
        /// <param name="_position"></param>
        public void RackPositionSave(Vector3 _position)
        {
            equipment_Position = _position;
        }

        /// <summary>
        /// Module 오브젝트에 대한 Property 값 저장
        /// </summary>
        /// <param name="_data"></param>
        public void ModulePropertyInit(ModuleEquipmentData _data)
        {
            equipment_id = _data.device_Id;
            equipment_Name = _data.module_Name;
            equipment_Type = _data.module_Type;
            if (_data.module_Unit.Equals(EquipmentUnit.Module.ToString()))
            {
                equipment_Unit = EquipmentUnit.Module;
            }
            moduleProperty.SetCellGroupCount(_data.cellGroup_Count);
            moduleProperty.SetCellGroupCol(_data.cellGroup_col);
        }

        /// <summary>
        /// 생성된 Module의 Oringnal Position값 저장
        /// </summary>
        /// <param name="_position"></param>
        public void ModulePositionSave(Vector3 _position)
        {
            equipment_Position = _position;
        }

        public void CellGroupPropertyInit(CellGroupEquipmentData _data)
        {
            equipment_id = _data.device_Id;
            equipment_Name = _data.cellGroup_Name;
            equipment_Type = _data.cellGroup_Type;
            if (_data.cellGroup_Unit.Equals(EquipmentUnit.CellGrounp.ToString()))
            {
                equipment_Unit = EquipmentUnit.CellGrounp;
            }
        }
        #endregion
    }
}
