using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BitmapToBytes
{

    public class XBMGenerator
    {
        private string _inputFilename;
        public string ErrorResult;

        public XBMGenerator(string inputFileName)
        {
            _inputFilename = inputFileName;
        }

        public ConversionResult Process()
        {
            var result = new ConversionResult();

            Bitmap myImg = (Bitmap)Bitmap.FromFile(_inputFilename);
            result.ImageWidth = myImg.Width;
            result.ImageHeight = myImg.Height;

            if (myImg.Width % 8 == 0)
            {
                StringBuilder sb = new StringBuilder();
                //sb.Append("{" + Environment.NewLine);
                StringBuilder sbLine = new StringBuilder();
                string sLine = "";
                for (int ii = 0; ii < myImg.Height; ii++)
                {
                    // loop each row of image
                    for (int jj = 0; jj < myImg.Width; jj++)
                    {
                        // loop each pixel in row and add to sbLine string to build
                        // string of bits for black and white image
                        Color pixelColor = myImg.GetPixel(jj, ii);
                        //sbLine.Append(HexConverter(pixelColor));
                        sLine = HexConverter(pixelColor) + sLine;
                        if (sLine.Count() == 8) // for our OLED display, the byte bits are reversed
                        {
                            sbLine.Append(sLine);
                            sLine = "";
                        }
                    }
                    // convert sbline string to byte array
                    byte[] buffer = GetBytes(sbLine.ToString());
                    //byte[] buffer = GetBytes(sLine);
                    // add first 0x to output row
                    sb.Append("0x");
                    // convert byte array to hex
                    sb.Append(BitConverter.ToString(buffer).Replace("-", ",0x"));
                    // clear line data
                    sbLine.Clear();
                    sLine = "";
                    buffer = null;
                    // add comma to end of row
                    sb.Append(",");
                    // add new line
                    sb.Append(Environment.NewLine);
                }
                // write output to screen

                //sb.Append("};" + Environment.NewLine);
                //textBox1.Text = sb.ToString();
                result.Success = true;
                result.Data = sb.ToString();
            }
            else
            {
                // image isnt multiple of 8 wide
                //MessageBox.Show("Image is not a multiple of 8 pixels wide");
                result.ErrorMessage = "Image is not a multiple of 8 pixels wide";
            }

            return result;
        }



        public byte[] GetBytes(string bitString)
        {
            return Enumerable.Range(0, bitString.Length / 8).
                Select(pos => Convert.ToByte(
                    bitString.Substring(pos * 8, 8),
                    2)
                ).ToArray();
        }

        private String HexConverter(System.Drawing.Color c)
        {
            if ((c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2")).Equals("000000"))
            {
                // image black pixel
                return "1";
            }
            else
            {
                // image is not a black pixel
                return "0";
            }
        }

    }
}
