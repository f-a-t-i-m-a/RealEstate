using System;
using System.Collections.Generic;
using System.IO;
using Compositional.Composer;
using JahanJooy.Common.Util.Configuration;

namespace JahanJooy.Common.Util.Components
{
    [Contract]
    [Component]
    public class BinaryStorageComponent
    {
        private readonly HashSet<string> _registeredStoreIds = new HashSet<string>();

        [ComponentPlug]
        public IApplicationSettings ApplicationSettings { get; set; }

        public static void RegisterStoreConfigurationKeys(string storeId)
        {
            ApplicationSettingKeys.RegisterKey(GetSettingKeyForStorePath(storeId));
            ApplicationSettingKeys.RegisterKey(GetSettingKeyForStoreFileExtension(storeId));
        }

        public void RegisterStoreId(string storeId)
        {
            if (_registeredStoreIds.Contains(storeId))
                return;

            _registeredStoreIds.Add(storeId);
        }

        public bool CheckStoreIdValid(string storeId)
        {
            return _registeredStoreIds.Contains(storeId);
        }

        public bool CheckStoreExists(string storeId)
        {
            EnsureStoreIdValid(storeId);
            return Directory.Exists(GetStorePath(storeId));
        }

        public long GetMaxPartitionId(string storeId)
        {
            EnsureStoreIdValid(storeId);
            return Byte.MaxValue;
        }

        public bool CheckPartitionIdValid(string storeId, long partitionId)
        {
            return partitionId >= 0 && partitionId <= GetMaxPartitionId(storeId);
        }

        public bool CheckPartitionExists(string storeId, long partitionId)
        {
            return Directory.Exists(BuildPartitionPath(storeId, partitionId));
        }

        public Guid[] List(string storeId, long partitionId)
        {
            var partitionPath = BuildPartitionPath(storeId, partitionId);
            if (!Directory.Exists(partitionPath))
                return new Guid[0];

            var result = new List<Guid>();
            foreach (var itemFilePath in Directory.GetFiles(partitionPath))
            {
                Guid itemId;
                if (Guid.TryParseExact(Path.GetFileNameWithoutExtension(itemFilePath), "N", out itemId))
                    result.Add(itemId);
            }

            return result.ToArray();
        }

        public bool CheckItemExists(string storeId, Guid itemId)
        {
            return File.Exists(CalculateItemFilePath(storeId, itemId, false));
        }

        public long GetItemSize(string storeId, Guid itemId)
        {
            var itemFilePath = CalculateItemFilePath(storeId, itemId, false);
            EnsureItemExists(itemFilePath, storeId, itemId);

            var itemFileInfo = new FileInfo(itemFilePath);
            return itemFileInfo.Length;
        }

        public DateTime GetItemLastAccessTime(string storeId, Guid itemId)
        {
            var itemFilePath = CalculateItemFilePath(storeId, itemId, false);
            EnsureItemExists(itemFilePath, storeId, itemId);

            var itemFileInfo = new FileInfo(itemFilePath);
            return itemFileInfo.LastAccessTime;
        }

        public DateTime GetItemLastModificationTime(string storeId, Guid itemId)
        {
            var itemFilePath = CalculateItemFilePath(storeId, itemId, false);
            EnsureItemExists(itemFilePath, storeId, itemId);

            var itemFileInfo = new FileInfo(itemFilePath);
            return itemFileInfo.LastWriteTime;
        }

        public void StoreBytes(string storeId, Guid itemId, byte[] blob)
        {
            var itemFilePath = CalculateItemFilePath(storeId, itemId, true);
            File.WriteAllBytes(itemFilePath, blob);
        }

        public byte[] RetrieveBytes(string storeId, Guid itemId)
        {
            var itemFilePath = CalculateItemFilePath(storeId, itemId, false);
            EnsureItemExists(itemFilePath, storeId, itemId);

            return File.ReadAllBytes(itemFilePath);
        }

        public void DeleteItem(string storeId, Guid itemId)
        {
            var itemFilePath = CalculateItemFilePath(storeId, itemId, false);
            EnsureItemExists(itemFilePath, storeId, itemId);

            File.Delete(itemFilePath);
        }

        #region Private helper methods

        private void EnsureStoreIdValid(string storeId)
        {
            if (!CheckStoreIdValid(storeId))
                throw new ArgumentException("Store ID " + storeId + " is not configured.");
        }

        private void EnsurePartitionIdValid(string storeId, long partitionId)
        {
            if (!CheckPartitionIdValid(storeId, partitionId))
                throw new ArgumentException("Store ID " + storeId + " does not have partition ID " + partitionId.ToString("X"));
        }

        private void EnsureItemExists(string itemFilePath, string storeId, Guid itemId)
        {
            if (!File.Exists(itemFilePath))
                throw new ArgumentException("Item " + itemId.ToString("N") + " does not exist in store " + storeId + ".");
        }

        private string BuildPartitionPath(string storeId, long partitionId)
        {
            EnsurePartitionIdValid(storeId, partitionId);
            return Path.Combine(GetStorePath(storeId), partitionId.ToString("X2"));
        }

        private long CalculatePartitionId(string storeId, Guid itemId)
        {
            var itemIdBytes = itemId.ToByteArray();
            return itemIdBytes[itemIdBytes.Length - 1];
        }

        private string CalculateItemFileName(string storeId, Guid itemId)
        {
            EnsureStoreIdValid(storeId);
            return itemId.ToString("N") + "." + GetStoreFileExtension(storeId);
        }

        private string CalculateItemFilePath(string storeId, Guid itemId, bool createFolder)
        {
            long partitionId = CalculatePartitionId(storeId, itemId);
            var partitionPath = BuildPartitionPath(storeId, partitionId);

            if (createFolder && !Directory.Exists(partitionPath))
                Directory.CreateDirectory(partitionPath);

            string itemFileName = CalculateItemFileName(storeId, itemId);
            return Path.Combine(partitionPath, itemFileName);
        }

        private static string GetSettingKeyForStorePath(string storeId)
        {
            return "BinaryStore." + storeId + ".Path";
        }

        private static string GetSettingKeyForStoreFileExtension(string storeId)
        {
            return "BinaryStore." + storeId + ".FileExtension";
        }

        private string GetStorePath(string storeId)
        {
            return ApplicationSettings[GetSettingKeyForStorePath(storeId)];
        }

        private string GetStoreFileExtension(string storeId)
        {
            return ApplicationSettings[GetSettingKeyForStoreFileExtension(storeId)];
        }

        #endregion
    }
}