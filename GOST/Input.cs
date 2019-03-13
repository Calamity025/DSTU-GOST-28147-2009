using System;
using System.Collections;
using System.Text;

namespace GOST
{
    public enum mode
    {
        SimpleSub,
        Gamma,
        BackGamma
    }
    
    public class Input
    {
        private BitArray input;
        
        public Input(string plain)
        {
            int length = plain.Length;
            BitArray temp = new BitArray(Encoding.GetEncoding(866).GetBytes(plain));
            if(length % 8==0)
            {
                input = temp;
                input = Calculus.Reverse(input);
            }
            else
            {
                input = new BitArray(length * 8 + (64 - (length % 8) * 8));
                for (int i = 0; i < length * 8; i++)
                {
                    input[i] = temp[length * 8 - i - 1];
                }
                SimpleSub(length*8);
            }
        }
        
        private void SimpleSub(int infoLength) {
            for (int i = infoLength; i < input.Length - 8; i++)
            {
                input.Set(i, true);
            }
            BitArray temp = Calculus.Reverse(new BitArray( new []{(byte)(input.Length-infoLength)}));
            for (int i = input.Length - 8; i < input.Length; i++)
            {
                input.Set(i, temp.Get(i%8));
            }
        }
    }
}