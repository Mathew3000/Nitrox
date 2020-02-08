using NitroxModel.DataStructures.Util;
using NitroxModel.Logger;
using NitroxServer.GameLogic;
using NitroxServer.GameLogic.Bases;
using NitroxServer.GameLogic.Entities;
using NitroxServer.GameLogic.Entities.Spawning;
using NitroxServer.GameLogic.Items;
using NitroxServer.GameLogic.Players;
using NitroxServer.GameLogic.Vehicles;
using System;
using System.Collections.Generic;
using System.IO;
using NitroxServer.GameLogic.Unlockables;
using NitroxServer.ConfigParser;
using NitroxModel.DataStructures;
using NitroxModel.Core;
using NitroxModel.DataStructures.GameLogic.Entities;
using NitroxServer.GameLogic.Entities.EntityBootstrappers;
using NitroxServer.Serialization.Resources.Datastructures;
using NitroxModel.DataStructures.GameLogic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace NitroxServer.Serialization.World
{
    public class NitroxDebug
    {
        public const string debugSaveFile = "dbg";


        #region SerializationHelpers
        [Serializable]
        public class EntitySerializationHelper
        {

            public string IdStr;

            public float pos_x;
            public float pos_y;
            public float pos_z;

            public float rot_x;
            public float rot_y;
            public float rot_z;
            public float rot_w;

            public float sca_x;
            public float sca_y;
            public float sca_z;

            public string techString;

            public string id_str;

            public int lvl;

            public string classId_str;

            public List<EntitySerializationHelper> Child_Entities;

            public bool serverSpawned;

            public string waterParkIdStr;

            public byte[] serializedGO;

            public bool InGlobalRoot;

            public EntitySerializationHelper()
            {

            }

            /// <summary>
            /// Converts Entity kvp to serializable format
            /// </summary>
            /// <param name="_entity"></param>
            public EntitySerializationHelper(KeyValuePair<NitroxId, Entity> kvp)
            {
                // New Children
                Child_Entities = new List<EntitySerializationHelper>();

                // NitroxId (key)
                IdStr = kvp.Key.ToString();
                // Entity (value)
                // Position
                pos_x = kvp.Value.Position.x;
                pos_y = kvp.Value.Position.y;
                pos_z = kvp.Value.Position.z;
                // Rotation
                rot_x = kvp.Value.Rotation.x;
                rot_y = kvp.Value.Rotation.y;
                rot_z = kvp.Value.Rotation.z;
                rot_w = kvp.Value.Rotation.w;
                // Scale
                sca_x = kvp.Value.Scale.x;
                sca_y = kvp.Value.Scale.y;
                sca_z = kvp.Value.Scale.z;
                // TechType
                techString = kvp.Value.TechType.Name;
                // Id
                id_str = kvp.Value.Id.ToString();
                // Level
                lvl = kvp.Value.Level;
                // ClassId
                classId_str = kvp.Value.ClassId;
                // Children
                if (kvp.Value.ChildEntities != null)
                {
                    foreach (Entity child in kvp.Value.ChildEntities)
                    {
                        Child_Entities.Add(new EntitySerializationHelper(child));
                    }
                }
                // Spawned by server
                serverSpawned = kvp.Value.SpawnedByServer;
                // WaterParkId
                if (kvp.Value.WaterParkId != null)
                {
                    waterParkIdStr = kvp.Value.WaterParkId.ToString();
                }
                // SerializedObject
                if (kvp.Value.SerializedGameObject != null)
                {
                    serializedGO = kvp.Value.SerializedGameObject;
                }
                // ExistInGlobal
                InGlobalRoot = kvp.Value.ExistsInGlobalRoot;

            }

            /// <summary>
            /// Converts entity to serializable format
            /// </summary>
            /// <param name="entity"></param>
            public EntitySerializationHelper(Entity entity)
            {
                // New Children
                Child_Entities = new List<EntitySerializationHelper>();
                // Position
                pos_x = entity.Position.x;
                pos_y = entity.Position.y;
                pos_z = entity.Position.z;
                // Rotation
                rot_x = entity.Rotation.x;
                rot_y = entity.Rotation.y;
                rot_z = entity.Rotation.z;
                rot_w = entity.Rotation.w;
                // Scale
                sca_x = entity.Scale.x;
                sca_y = entity.Scale.y;
                sca_z = entity.Scale.z;
                // TechType
                techString = entity.TechType.Name;
                // Id
                id_str = entity.Id.ToString();
                // Level
                lvl = entity.Level;
                // ClassId
                classId_str = entity.ClassId;
                // Children
                if (entity.ChildEntities != null)
                {
                    foreach (Entity child in entity.ChildEntities)
                    {
                        Child_Entities.Add(new EntitySerializationHelper(child));
                    }
                }
                // Spawned by server
                serverSpawned = entity.SpawnedByServer;
                // WaterParkId
                if (entity.WaterParkId != null)
                {
                    waterParkIdStr = entity.WaterParkId.ToString();
                }
                // SerializedObject
                if (entity.SerializedGameObject != null)
                {
                    serializedGO = entity.SerializedGameObject;
                }
                // ExistInGlobal
                InGlobalRoot = entity.ExistsInGlobalRoot;
            }

            public KeyValuePair<NitroxId, Entity> ToKVP()
            {
                KeyValuePair<NitroxId, Entity> kvp = new KeyValuePair<NitroxId, Entity>(new NitroxId(IdStr), ToEntity());

                return kvp;
            }

            public Entity ToEntity()
            {
                Entity ent = new Entity();

                ent.Position = new UnityEngine.Vector3(pos_x, pos_y, pos_z);
                ent.Rotation = new UnityEngine.Quaternion(rot_x, rot_y, rot_z, rot_w);
                ent.Scale = new UnityEngine.Vector3(sca_x, sca_y, sca_z);
                ent.TechType = new NitroxModel.DataStructures.TechType(techString);
                ent.Id = new NitroxId(id_str);
                ent.Level = lvl;
                ent.ClassId = classId_str;
                // Children
                if (Child_Entities != null)
                {
                    foreach (EntitySerializationHelper entitySerializationHelper in Child_Entities)
                    {
                        ent.ChildEntities.Add(entitySerializationHelper.ToEntity());
                    }
                }
                ent.SpawnedByServer = serverSpawned;
                if (waterParkIdStr != null)
                {
                    ent.WaterParkId = new NitroxId(waterParkIdStr);
                }
                if (serializedGO != null)
                {
                    ent.SerializedGameObject = serializedGO;
                }
                ent.ExistsInGlobalRoot = InGlobalRoot;

                return ent;

            }
        }

        [Serializable]
        public class ItemSerializationHelper
        {
            public string IdStr;
            public string ContainerIdStr;
            public string ItemIdStr;
            public byte[] SerializedData;

            public ItemSerializationHelper()
            {

            }

            public ItemSerializationHelper(KeyValuePair<NitroxId, ItemData> kvp)
            {
                // Key
                IdStr = kvp.Key.ToString();
                // Value
                // Container Id
                ContainerIdStr = kvp.Value.ContainerId.ToString();
                // Item Id
                ItemIdStr = kvp.Value.ItemId.ToString();
                // Serialized data
                SerializedData = kvp.Value.SerializedData;
            }

            public ItemSerializationHelper(ItemData dat)
            {
                // Key
                IdStr = dat.ToString();
                // Value
                // Container Id
                ContainerIdStr = dat.ContainerId.ToString();
                // Item Id
                ItemIdStr = dat.ItemId.ToString();
                // Serialized data
                SerializedData = dat.SerializedData;
            }

            public KeyValuePair<NitroxId, ItemData> ToKVP()
            {
                KeyValuePair<NitroxId, ItemData> kvp = new KeyValuePair<NitroxId, ItemData>(new NitroxId(IdStr), ToItemData());

                return kvp;
            }

            public ItemData ToItemData()
            {
                ItemData dat = new ItemData(new NitroxId(ContainerIdStr), new NitroxId(ItemIdStr), SerializedData);

                return dat;
            }
        }

        [Serializable]
        public class BaseSerializationHelper
        {
            public string IdStr;

            public string PieceId;

            public float ItemPosition_x;
            public float ItemPosition_y;
            public float ItemPosition_z;

            public float Rotation_x;
            public float Rotation_y;
            public float Rotation_z;
            public float Rotation_w;

            public string TechTypeStr;

            public string ParentBaseIdStr;

            public float CameraPosition_x;
            public float CameraPosition_y;
            public float CameraPosition_z;

            public float CameraRotation_x;
            public float CameraRotation_y;
            public float CameraRotation_z;
            public float CameraRotation_w;

            public float ConstrAmount;

            public bool ConstrCompleted;

            public bool IsFurniture;

            public string BaseIdStr;

            public BaseSerializationHelper()
            {

            }

            public BaseSerializationHelper(KeyValuePair<NitroxId, BasePiece> kvp)
            {
                // NitroxId (key)
                IdStr = kvp.Key.ToString();
                // BasePiece (value)
                // PieceId
                PieceId = kvp.Value.Id.ToString();
                // Position
                ItemPosition_x = kvp.Value.ItemPosition.x;
                ItemPosition_y = kvp.Value.ItemPosition.y;
                ItemPosition_z = kvp.Value.ItemPosition.z;
                // Rotation
                Rotation_x = kvp.Value.Rotation.x;
                Rotation_y = kvp.Value.Rotation.y;
                Rotation_z = kvp.Value.Rotation.z;
                Rotation_w = kvp.Value.Rotation.w;
                // TechType
                TechTypeStr = kvp.Value.TechType.Name;
                // ParentBaseId
                ParentBaseIdStr = kvp.Value.SerializableParentBaseId?.ToString();
                // CameraPosition
                CameraPosition_x = kvp.Value.CameraPosition.x;
                CameraPosition_y = kvp.Value.CameraPosition.y;
                CameraPosition_z = kvp.Value.CameraPosition.z;
                // Camera Rotation
                CameraRotation_x = kvp.Value.CameraRotation.x;
                CameraRotation_y = kvp.Value.CameraRotation.y;
                CameraRotation_z = kvp.Value.CameraRotation.z;
                CameraRotation_w = kvp.Value.CameraRotation.w;
                // Construction Perc
                ConstrAmount = kvp.Value.ConstructionAmount;
                // Construction Done
                ConstrCompleted = kvp.Value.ConstructionCompleted;
                // Furniture
                IsFurniture = kvp.Value.IsFurniture;
                // BaseId
                BaseIdStr = kvp.Value.BaseId?.ToString();
                // Rotation Metadata
                // Metadata

            }


            public BaseSerializationHelper(BasePiece basePiece)
            {
                // PieceId
                PieceId = basePiece.Id.ToString();
                // Position
                ItemPosition_x = basePiece.ItemPosition.x;
                ItemPosition_y = basePiece.ItemPosition.y;
                ItemPosition_z = basePiece.ItemPosition.z;
                // Rotation
                Rotation_x = basePiece.Rotation.x;
                Rotation_y = basePiece.Rotation.y;
                Rotation_z = basePiece.Rotation.z;
                Rotation_w = basePiece.Rotation.w;
                // TechType
                TechTypeStr = basePiece.TechType.Name;
                // ParentBaseId
                ParentBaseIdStr = basePiece.SerializableParentBaseId?.ToString();
                // CameraPosition
                CameraPosition_x = basePiece.CameraPosition.x;
                CameraPosition_y = basePiece.CameraPosition.y;
                CameraPosition_z = basePiece.CameraPosition.z;
                // Camera Rotation
                CameraRotation_x = basePiece.CameraRotation.x;
                CameraRotation_y = basePiece.CameraRotation.y;
                CameraRotation_z = basePiece.CameraRotation.z;
                CameraRotation_w = basePiece.CameraRotation.w;
                // Construction Perc
                ConstrAmount = basePiece.ConstructionAmount;
                // Construction Done
                ConstrCompleted = basePiece.ConstructionCompleted;
                // Furniture
                IsFurniture = basePiece.IsFurniture;
                // BaseId
                BaseIdStr = basePiece.BaseId?.ToString();
                // Rotation Metadata
                // Metadata
            }


            public KeyValuePair<NitroxId, BasePiece> ToKVP()
            {
                KeyValuePair<NitroxId, BasePiece> kvp = new KeyValuePair<NitroxId, BasePiece>(new NitroxId(IdStr), ToBasePiece());

                return kvp;
            }

            public BasePiece ToBasePiece()
            {
                BasePiece basePiece = new BasePiece();

                basePiece.Id = new NitroxId(PieceId);
                basePiece.ItemPosition = new UnityEngine.Vector3(ItemPosition_x, ItemPosition_y, ItemPosition_z);
                basePiece.Rotation = new UnityEngine.Quaternion(Rotation_x, Rotation_y, Rotation_z, Rotation_w);
                basePiece.TechType = new NitroxModel.DataStructures.TechType(TechTypeStr);
                basePiece.SerializableParentBaseId = new NitroxId(ParentBaseIdStr);
                basePiece.CameraPosition = new UnityEngine.Vector3(CameraPosition_x, CameraPosition_y, CameraPosition_z);
                basePiece.CameraRotation = new UnityEngine.Quaternion(CameraRotation_x, CameraRotation_y, CameraRotation_z, CameraRotation_w);
                basePiece.ConstructionAmount = ConstrAmount;
                basePiece.ConstructionCompleted = ConstrCompleted;
                basePiece.IsFurniture = IsFurniture;
                basePiece.BaseId = new NitroxId(BaseIdStr);

                return basePiece;
            }

        }

        #endregion




        public void SaveEntity(World world)
        {
            //BinaryFormatter entitySerializer = new BinaryFormatter();
            XmlSerializer entitySerializer = new XmlSerializer(typeof(EntitySerializationHelper[]));

            List<EntitySerializationHelper> entities = new List<EntitySerializationHelper>();

            foreach (KeyValuePair<NitroxId, Entity> kvp in world.EntityData.SerializableEntities)
            {
                entities.Add(new EntitySerializationHelper(kvp));
            }

            try
            {
                using (Stream stream = File.OpenWrite(debugSaveFile + ".nitroxEntity"))
                {
                    entitySerializer.Serialize(stream, entities.ToArray());
                }

                Log.Info("World Entities saved.");
            }
            catch (Exception ex)
            {
                Log.Info("Could not save Entities: " + ex);
            }
        }

        public void SaveBatchCells(World world)
        {
            XmlSerializer batchSerializer = new XmlSerializer(typeof(List<Int3>));

            try
            {
                using (Stream stream = File.OpenWrite(debugSaveFile + ".nitroxBatch"))
                {
                    batchSerializer.Serialize(stream, world.BatchEntitySpawner.SerializableParsedBatches);
                }

                Log.Info("World Batch saved.");
            }
            catch (Exception ex)
            {
                Log.Info("Could not save Batches: " + ex);
            }
        }

        public void SaveInventory(World world)
        {
            XmlSerializer inventorySerializer = new XmlSerializer(typeof(ItemSerializationHelper[]));

            // Inventory items are split in two
            List<ItemSerializationHelper> inventoryItems = new List<ItemSerializationHelper>();
            List<ItemSerializationHelper> slotItems = new List<ItemSerializationHelper>();

            foreach (KeyValuePair<NitroxId, ItemData> kvp in world.InventoryData.SerializableInsertedInventoryItemsById)
            {
                inventoryItems.Add(new ItemSerializationHelper(kvp));
            }
            foreach (KeyValuePair<NitroxId, ItemData> kvp in world.InventoryData.SerializableStorageSlotItemsById)
            {
                slotItems.Add(new ItemSerializationHelper(kvp));
            }
            try
            {

                // Write to two files
                using (Stream stream = File.OpenWrite(debugSaveFile + ".nitroxInventory"))
                {
                    inventorySerializer.Serialize(stream, inventoryItems.ToArray());
                }
                using (Stream stream = File.OpenWrite(debugSaveFile + ".nitroxSlots"))
                {
                    inventorySerializer.Serialize(stream, slotItems.ToArray());
                }
                Log.Info("World Inventories saved.");
            }
            catch (Exception ex)
            {
                Log.Info("Could not save Inventories: " + ex);
            }

        }


        public void SaveBase(World world)
        {
            XmlSerializer baseSerializer = new XmlSerializer(typeof(BaseSerializationHelper[]));

            // Split in Pieces and History
            List<BaseSerializationHelper> basePieces = new List<BaseSerializationHelper>();
            List<BaseSerializationHelper> baseHistory = new List<BaseSerializationHelper>();

            foreach (KeyValuePair<NitroxId, BasePiece> kvp in world.BaseData.SerializableBasePiecesById)
            {
                basePieces.Add(new BaseSerializationHelper(kvp));
            }
            foreach (BasePiece piece in world.BaseData.SerializableCompletedBasePieceHistory)
            {
                baseHistory.Add(new BaseSerializationHelper(piece));
            }

            try
            {
                // Write two files
                using (Stream stream = File.OpenWrite(debugSaveFile + ".nitroxBase"))
                {
                    baseSerializer.Serialize(stream, basePieces.ToArray());
                }
                using (Stream stream = File.OpenWrite(debugSaveFile + ".nitroxBaseHist"))
                {
                    baseSerializer.Serialize(stream, baseHistory.ToArray());
                }

                Log.Info("World Base saved.");
            }
            catch (Exception ex)
            {
                Log.Info("Could not save Base: " + ex);
            }
        }

        public EntityData LoadEntity()
        {
            EntityData entityData = new EntityData();

            List<EntitySerializationHelper> entities = new List<EntitySerializationHelper>();

            XmlSerializer entitySerializer = new XmlSerializer(typeof(EntitySerializationHelper[]));

            try
            {
                using (Stream stream = File.OpenRead(debugSaveFile + ".nitroxEntity"))
                {
                    entities.AddRange((EntitySerializationHelper[])(entitySerializer.Deserialize(stream)));
                }

                foreach (EntitySerializationHelper serializationHelper in entities)
                {
                    entityData.AddEntity(serializationHelper.ToEntity());
                }

                Log.Info("World Entities loaded.");
            }
            catch (Exception ex)
            {
                Log.Info("World Entities not loaded." + ex);
            }


            return entityData;
        }

        public List<Int3> LoadBatchCells()
        {
            List<Int3> batches = new List<Int3>();

            XmlSerializer batchSerializer = new XmlSerializer(typeof(List<Int3>));

            try
            {
                using (Stream stream = File.OpenRead(debugSaveFile + ".nitroxBatch"))
                {
                    batches = (List<Int3>)(batchSerializer.Deserialize(stream));
                }

                Log.Info("World BatchCells loaded.");
            }
            catch (Exception ex)
            {
                Log.Info("World BatchCells not loaded." + ex);
            }

            return batches;
        }

        public InventoryData LoadInventory()
        {
            XmlSerializer inventorySerializer = new XmlSerializer(typeof(ItemSerializationHelper[]));

            // Inventory items are split in two
            List<ItemSerializationHelper> inventoryItems = new List<ItemSerializationHelper>();
            List<ItemSerializationHelper> slotItems = new List<ItemSerializationHelper>();

            InventoryData inventoryData = new InventoryData();

            try
            {
                using (Stream stream = File.OpenRead(debugSaveFile + ".nitroxInventory"))
                {
                    inventoryItems.AddRange((ItemSerializationHelper[])(inventorySerializer.Deserialize(stream)));
                }
                using (Stream stream = File.OpenRead(debugSaveFile + ".nitroxSlots"))
                {
                    slotItems.AddRange((ItemSerializationHelper[])(inventorySerializer.Deserialize(stream)));
                }

                Dictionary<NitroxId, ItemData> InventoryItemsById = new Dictionary<NitroxId, ItemData>();
                foreach (ItemSerializationHelper serializationHelper in inventoryItems)
                {
                    KeyValuePair<NitroxId, ItemData> kvp = serializationHelper.ToKVP();

                    InventoryItemsById.Add(kvp.Key, kvp.Value);
                }
                Dictionary<NitroxId, ItemData> SlotItemsById = new Dictionary<NitroxId, ItemData>();
                foreach (ItemSerializationHelper serializationHelper in slotItems)
                {
                    KeyValuePair<NitroxId, ItemData> kvp = serializationHelper.ToKVP();

                    SlotItemsById.Add(kvp.Key, kvp.Value);
                }

                inventoryData.SerializableInsertedInventoryItemsById = InventoryItemsById;
                inventoryData.SerializableStorageSlotItemsById = SlotItemsById;

                Log.Info("World Inventories loaded.");
            }
            catch (Exception ex)
            {
                Log.Info("World Inventories not loaded." + ex);
            }

            return inventoryData;
        }


        public BaseData LoadBase()
        {
            XmlSerializer baseSerializer = new XmlSerializer(typeof(BaseSerializationHelper[]));

            // Split in Pieces and History
            List<BaseSerializationHelper> basePieces = new List<BaseSerializationHelper>();
            List<BaseSerializationHelper> baseHistory = new List<BaseSerializationHelper>();

            BaseData baseData = new BaseData();

            try
            {
                using (Stream stream = File.OpenRead(debugSaveFile + ".nitroxBase"))
                {
                    basePieces.AddRange((BaseSerializationHelper[])(baseSerializer.Deserialize(stream)));
                }
                using (Stream stream = File.OpenRead(debugSaveFile + ".nitroxBaseHist"))
                {
                    baseHistory.AddRange((BaseSerializationHelper[])(baseSerializer.Deserialize(stream)));
                }

                Dictionary<NitroxId, BasePiece> BasePiecesById = new Dictionary<NitroxId, BasePiece>();
                foreach (BaseSerializationHelper serializationHelper in basePieces)
                {
                    KeyValuePair<NitroxId, BasePiece> kvp = serializationHelper.ToKVP();

                    BasePiecesById.Add(kvp.Key, kvp.Value);
                }
                List<BasePiece> PiecesHistory = new List<BasePiece>();
                foreach (BaseSerializationHelper serializationHelper in baseHistory)
                {
                    BasePiece dat = serializationHelper.ToBasePiece();

                    PiecesHistory.Add(dat);
                }

                baseData.SerializableBasePiecesById = BasePiecesById;
                baseData.SerializableCompletedBasePieceHistory = PiecesHistory;

                Log.Info("World Bases loaded.");
            }
            catch (Exception ex)
            {
                Log.Info("World Bases not loaded." + ex);
            }

            return baseData;
        }




    }
}
