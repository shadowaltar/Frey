using System.Data;
using Maintenance.Common;
using Maintenance.Common.Data;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;

namespace Maintenance.AssetMaps
{
    public class AssetMapDataAccess : ImapDataAccess
    {
        internal DataTable GetAllAssetMapComponentProperties()
        {
            return Query(@"
SELECT * 
FROM ASM_COMPONENT_PROPERTY ACP 
JOIN ASM_COMPONENT AC 
ON AC.ASM_COMP_ID = ACP.ASM_COMP_ID");
        }

        internal DataTable GetCbItemAffectedAsmCompIds()
        {
            return Query(@"SELECT DISTINCT ASM_COMP_ID ACID FROM COMPOSITE_BENCHMARK_ITEM");
        }

        internal DataTable GetNewAssetMapId()
        {
            return Query(@"SELECT IMAP_MAPS.SEQ_ASM_ID.NEXTVAL FROM DUAL");
        }

        internal DataTable GetNewPropertyIds(int count)
        {
            return Query(@"
SELECT IMAP_MAPS.SEQ_ASM_COMP_PROP_ID.NEXTVAL 
FROM (SELECT LEVEL FROM DUAL CONNECT BY LEVEL < {0})", count);
        }

        internal DataTable GetNewComponentIds(int count)
        {
            return Query(@"
SELECT IMAP_MAPS.SEQ_ASM_COMP_ID.NEXTVAL 
FROM (SELECT LEVEL FROM DUAL CONNECT BY LEVEL < {0})", count);
        }

        internal bool InsertAssetMapComponent(AssetMapComponent component)
        {
            var rowCount = Execute(@"
INSERT INTO ASM_COMPONENT 
(ASM_COMP_ID, ASM_ID, PARENT_ASM_COMP_ID, ASM_COMP_NAME, ASM_COMP_ORDER, ASM_COMP_CODE) 
VALUES ({0}, {1}, {2}, '{3}', {4}, '{5}')"
                , component.Id, component.AsmId,
                component.IsRoot ? "NULL" : component.ParentId.ConvertString(),
                component.Name, component.Order, component.Code);
            return rowCount == 1;
        }

        internal bool UpdateAssetMapComponent(AssetMapComponent component)
        {
            var rowCount = Execute(@"
UPDATE ASM_COMPONENT 
SET ASM_COMP_NAME = '{0}', ASM_COMP_ORDER = {1}, ASM_COMP_CODE = '{2}' WHERE ASM_COMP_ID = {3}"
                , component.Name, component.Order,
                component.Code, component.Id);
            return rowCount == 1;
        }

        internal bool DeleteAssetMapComponent(AssetMapComponent component)
        {
            var rowCount = Execute(@"DELETE ASM_COMPONENT WHERE ASM_COMP_ID = {0}", component.Id);
            return rowCount == 1;
        }

        internal bool InsertAssetMapComponentProperty(long assetMapComponentId,
            AssetMapComponentProperty property)
        {
            var rowCount = Execute(@"
INSERT INTO ASM_COMPONENT_PROPERTY 
(ASM_COMP_PROPERTY_ID, ASM_COMP_ID, PROP_KEY, PROP_VALUE, UPDATE_DATETIME, UPDATE_SOURCE) 
VALUES ({0}, {1}, '{2}', '{3}', TO_DATE('{4}','YYYY-MM-DD'), {5})"
                , property.Id, assetMapComponentId, property.Key, property.Value,
                property.UpdateTime.IsoFormat(), "NULL");
            return rowCount == 1;
        }

        internal bool UpdateAssetMapComponentProperty(AssetMapComponentProperty property)
        {
            var rowCount = Execute(@"
UPDATE ASM_COMPONENT_PROPERTY 
SET PROP_KEY = '{0}', PROP_VALUE = '{1}', UPDATE_DATETIME = SYSDATE, UPDATE_SOURCE = '{3}' 
WHERE ASM_COMP_PROPERTY_ID = {4}"
                , property.Key, property.Value, property.UpdateTime.IsoFormat(), Constants.UserName, property.Id);
            return rowCount == 1;
        }

        internal bool DeleteAssetMapComponentProperty(AssetMapComponentProperty property)
        {
            var rowCount = Execute(@"
DELETE ASM_COMPONENT_PROPERTY WHERE ASM_COMP_PROPERTY_ID = {0}", property.Id);
            return rowCount == 1;
        }

        internal bool UpdateAssetMap(AssetMap assetMap)
        {
            var rowCount = Execute(@"
UPDATE ASM_STRUCTURE SET ASM_NAME = '{0}', ASM_CODE = '{1}' WHERE ASM_ID = {2}"
                , assetMap.Name, assetMap.Code, assetMap.Id);
            return rowCount == 1;
        }

        internal bool InsertAssetMap(AssetMap assetMap)
        {
            var rowCount = Execute(@"
INSERT INTO ASM_STRUCTURE 
(ASM_ID, ASM_NAME, ASM_CODE, COMMENTS, ACTIVE_FLAG) SELECT {0},'{1}','{2}','{3}','{4}' FROM DUAL"
                , assetMap.Id, assetMap.Name, assetMap.Code, "Inserted by Asset Map Maintenance", "Y");
            return rowCount == 1;
        }

        internal bool UpdateRootComponent(AssetMap assetMap, long rootCompId)
        {
            var rowCount = Execute(@"
UPDATE ASM_STRUCTURE SET ROOT_COMP_ID = {0} 
WHERE ASM_CODE = '{1}' AND ASM_NAME = '{2}' AND ROOT_COMP_ID IS NULL"
                , rootCompId, assetMap.Code, assetMap.Name);
            return rowCount == 1;
        }

        internal bool DeleteAssetMap(AssetMap assetMap)
        {
            var rowCount = Execute(@"DELETE ASM_COMPONENT WHERE ASM_ID = {0}", assetMap.Id);
            if (rowCount == 0)
                return false;

            rowCount = Execute(@"DELETE ASM_STRUCTURE WHERE ASM_ID = {0}", assetMap.Id);
            return rowCount == 1;
        }
    }
}