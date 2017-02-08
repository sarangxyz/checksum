using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace checksum
{
    class Checksum
    {
        [Flags]
        public enum Algo
        {
            None  = 0,
            CRC32 = 1,
            MD5   = 2,
            SHA1  = 4,
            SHA256 = 8,
            SHA384 = 16,
            SHA512 = 32
        }

        public string CRC32 { get; private set; } = string.Empty;
        public string MD5 { get; private set; } = string.Empty;
        public string SHA1 { get; private set; } = string.Empty;
        public string SHA256 { get; private set; } = string.Empty;
        public string SHA384 { get; private set; } = string.Empty;
        public string SHA512 { get; private set; } = string.Empty;

        public Checksum()
        { }

        public static Checksum Evaluate(Checksum.Algo algo, System.IO.FileStream fs)
        {
            Checksum eval = new Checksum();

            if ((algo & Algo.CRC32) == Algo.CRC32)
            {
                RewindStreamToStart(fs);
                eval.CRC32 = CalculateCRC32CheckSum(fs);
            }

            if((algo & Algo.MD5) == Algo.MD5)
            {
                RewindStreamToStart(fs);
                eval.MD5 = CalculateMD5CheckSum(fs);
            }

            if ((algo & Algo.SHA1) == Algo.SHA1)
            {
                RewindStreamToStart(fs);
                eval.SHA1 = CalculateSHA1CheckSum(fs);
            }

            if ((algo & Algo.SHA256) == Algo.SHA256)
            {
                RewindStreamToStart(fs);
                eval.SHA256 = CalculateSHA256CheckSum(fs);
            }

            if ((algo & Algo.SHA384) == Algo.SHA384)
            {
                RewindStreamToStart(fs);
                eval.SHA384 = CalculateSHA384CheckSum(fs);
            }

            if ((algo & Algo.SHA512) == Algo.SHA512)
            {
                RewindStreamToStart(fs);
                eval.SHA512 = CalculateSHA512CheckSum(fs);
            }

            return eval;
        }

        #region CHECKSUM_CALCULATION
        private static void RewindStreamToStart(FileStream fs)
        {
            //  note: you cannot reopen the stream once and be done
            //  CRC32 will go to the end of the stream and further checksum
            //  will just return same value for any file- cause we don't have
            //  anything to compute checksum from
            //  
            fs.Seek(0, SeekOrigin.Begin);
        }

        private static string CalculateCheckSum(FileStream fs, System.Security.Cryptography.HashAlgorithm algo)
        {
            string hashStr = string.Empty;
            byte[] hash = algo.ComputeHash(fs);
            hashStr = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            return hashStr;
        }

        private static string CalculateCRC32CheckSum(FileStream fs)
        {
            string crc32Hash = string.Empty;
            using (DamienG.Security.Cryptography.Crc32 crc32 = new DamienG.Security.Cryptography.Crc32())
                crc32Hash = CalculateCheckSum(fs, crc32);

            return crc32Hash;
        }


        private static string CalculateMD5CheckSum(FileStream fs)
        {
            string md5Hash = string.Empty;
            using (System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
                md5Hash = CalculateCheckSum(fs, md5);

            return md5Hash;
        }


        private static string CalculateSHA1CheckSum(FileStream fs)
        {
            string sha1Hash = string.Empty;
            using (System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create())
                sha1Hash = CalculateCheckSum(fs, sha1);
            return sha1Hash;
        }


        private static string CalculateSHA256CheckSum(FileStream fs)
        {
            string sha256Hash = string.Empty;
            using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
                sha256Hash = CalculateCheckSum(fs, sha256);
            return sha256Hash;
        }


        private static string CalculateSHA384CheckSum(FileStream fs)
        {
            string sha384Hash = string.Empty;
            using (System.Security.Cryptography.SHA384 sha384 = System.Security.Cryptography.SHA384.Create())
                sha384Hash = CalculateCheckSum(fs, sha384);
            return sha384Hash;
        }

        private static string CalculateSHA512CheckSum(FileStream fs)
        {
            string sha512Hash = string.Empty;
            using (System.Security.Cryptography.SHA512 sha512 = System.Security.Cryptography.SHA512.Create())
                sha512Hash = CalculateCheckSum(fs, sha512);
            return sha512Hash;
        }

        #endregion CHECKSUM_CALCULATION
    }
}
