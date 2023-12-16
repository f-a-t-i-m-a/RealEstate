using System.IO;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace JahanJooy.RealEstateAgency.Util.Attachments
{
    [Contract]
    [Component]
    public class PhotoGridStore
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        public GridFSBucket Grid { get; set; }

        public PhotoGridStore()
        {
            Grid = new GridFSBucket(DbManager.Database);
        }

        [OnCompositionComplete]
        public void OnCompositionComplete()
        {
        }

        public byte[] GetAttchmentBytes(AttachmentStoreEntityType type, ObjectId photoId, PhotoStoreSize size)
        {
            var storeName = type + "_" + size + "_" + photoId;
            return GetFile(storeName);
        }

        public short StoreAttachmentBytes(AttachmentStoreEntityType type, ObjectId photoId, PhotoStoreSize size,
            byte[] byteArray)
        {
            var storeName = type + "_" + size + "_" + photoId;
            return StoreFile(byteArray, storeName);
        }

        #region Private helper methods

        private short StoreFile(byte[] byteArray, string storeName)
        {
            var fileInfo = Grid.UploadFromBytesAsync(storeName, byteArray);
            return fileInfo.Result.Pid;
        }

        private byte[] GetFile(string storeName)
        {
            if (Grid == null)
                return new byte[0];

            lock (this)
            {
                try
                {
                    return Grid.DownloadAsBytesByNameAsync(storeName).Result;
                }
                catch (FileNotFoundException)
                {
                    return new byte[0];
                }
            }
        }

        #endregion
    }
}