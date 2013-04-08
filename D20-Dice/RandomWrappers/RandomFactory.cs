using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace D20_Dice.RandomWrappers
{
    public class RandomFactory
    {
        public static IRandomWrapper Create(Random random)
        {
            IRandomWrapper randomWrapper = new RandomWrapper(random);

            return randomWrapper;
        }
        
        public static IRandomWrapper CreateCryptographicallyStrong()
        {
            var crypto = new RNGCryptoServiceProvider();
            var cryptoByteBuffer = new Byte[4];
 
            crypto.GetBytes(cryptoByteBuffer);
            var random = new Random(BitConverter.ToInt32(cryptoByteBuffer, 0));

            return Create(random);
        }
    }
}