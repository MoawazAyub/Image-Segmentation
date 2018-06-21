using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace ConsoleApplication142
{
    class Program
    {
        static void Main(string[] args)
        {
            //Mat img10 = new Mat(@"E:\work\Projects\projects\ConsoleApplication142\ConsoleApplication142\img\BadHist.jpg",ImreadModes.Color);
            //img10.Set<int>(1,1);
            //new Window("Final", img10);

            Cv2.WaitKey();
            ///img1 = (0, 0, 255);
            //Mat ss = CvSet(img1, CV_RGB(redVal, greenVal, blueVal));

            Mat src = new Mat(@"E:\work\Projects\projects\ConsoleApplication142\ConsoleApplication142\img\check.tif", ImreadModes.GrayScale);
            int NoofSegments = 0;
            Random rnd = new Random();

            OpenCV CV = new OpenCV();
            Mat img = new Mat();
            img = CV.DoOpenCv(2,67,45,3000,20,src,false);
            NoofSegments++;
            //img = CV.DoOpenCv(0,0,0,3000,100,img,false);

            bool onetime = false;

            for (int i = 0; i < img.Rows; i++)
            {
                for (int j = 0; j < img.Cols; j++)
                {
                    int pix = img.At<byte>(i, j);
                    if (pix == 255)
                    {
                        NoofSegments++;
                        if (onetime == false)
                        {
                            img = CV.DoOpenCv(2, i, j, 1, (byte)rnd.Next(20, 200), src, false, true);
                            onetime = true;
                        }
                        else
                        {
                            img = CV.DoOpenCv(2, i, j, 1, (byte)rnd.Next(20, 200), src, true, false);
                        }

                    }

                }
            }


            //for (int i = 0; i < img.Rows; i++)
            //{
            //    for (int j = 0; j < img.Cols; j++)
            //    {
            //        int pix = img.At<byte>(i, j);
            //        if (pix == 255)
            //        {
            //            NoofSegments++;

            //            img = CV.DoOpenCv(0, i, j, 1, (byte)rnd.Next(20, 200), img, false,false);
            //        }

            //    }
            //}



            using (new Window("Final", img))
            {

                Console.WriteLine("\n\n\n\nTotal number of segments are ");
                Console.WriteLine(NoofSegments);
                Cv2.WaitKey();

            }
            
        


        }


    }

    class Pixel
    {
        public int PixelRow;
        public int PixelCol;

        public Pixel()
        {
            PixelRow = 0;
            PixelCol = 0;
        }

        public Pixel(int row,int col)
        {
            PixelRow = row;
            PixelCol = col;

        }

        
    }

    class OpenCV
    {
        public List<Pixel> PixelList = new List<Pixel>();
        int imgRows;
        int imgCols;
        Mat globalsegment = new Mat();


        public OpenCV()
        {
        }

        bool isInRange(int row,int col)
        {
            if (row >= 0 && row < imgRows && col >= 0 && col < imgCols)
            {
                return true;
            }

            return false;
        }

        public Mat DoOpenCv(int Thresh,int seedRow,int seedCol,int time,byte label,Mat src1,bool segmentrestore,bool initialize = true)
        {
            int ThreshHold = Thresh;

            int SeedRow = seedRow;
            int SeedCol = seedCol;

            int TempRow = 0;
            int TempCol = 0;


            //Mat src = new Mat(@"E:\work\Projects\projects\ConsoleApplication142\ConsoleApplication142\img\check.tif", ImreadModes.GrayScale);
            Mat src = src1.Clone();
            Mat SegmentationImg;
            if (segmentrestore == false)
            {
                SegmentationImg = src.Clone();
            }
            else {
                SegmentationImg = globalsegment;
            }

            imgRows = src.Rows;
            imgCols = src.Cols;



            Console.Write(imgRows);
            Console.Write(" ");
            Console.WriteLine(imgCols);

            //----------------------------------------intializing img
            if (initialize == true) {
                for (int i = 0; i < src.Rows; i++)
                {
                    for (int j = 0; j < src.Cols; j++)
                    {
                        SegmentationImg.Set<byte>(i, j, 255);
                    }

                }
            }

            //----------------------------------------output original img

            using (new Window("src image", src))
            {

                //new Window("Segmentation image", SegmentationImg);
                //Cv2.WaitKey();

            }

            //----------------------------------------------- Region Growing four point neighbour hood

            PixelList.Add(new Pixel(SeedRow,SeedCol));
            
            while (PixelList.Count != 0)
            {
                new Window("Segmented Image", SegmentationImg);
                //Cv2.WaitKey(1);

                TempRow = PixelList[0].PixelRow;
                TempCol = PixelList[0].PixelCol;

                //Console.WriteLine(PixelList.Count);

                int currPix = src.At<byte>(TempRow, TempCol);

              

                SegmentationImg.Set < byte >(TempRow,TempCol,label);
                PixelList.RemoveAt(0);
               // PixelList.RemoveAt(PixelList.Count - 1);
                //Console.WriteLine(PixelList.Count);
                //----------------------------------------upper
                if (isInRange(TempRow - 1,TempCol))
                {
                    int pix = src.At<byte>(TempRow - 1, TempCol);
                    int already = SegmentationImg.At<byte>(TempRow - 1, TempCol);
                    if (Math.Abs(pix - currPix) <= ThreshHold  && already == 255)
                    {
                        PixelList.Add(new Pixel(TempRow - 1, TempCol));
                        SegmentationImg.Set<byte>(TempRow - 1, TempCol, label);
                    }
                }
                //----------------------------------------lower
                if (isInRange(TempRow + 1, TempCol))
                {
                    int pix = src.At<byte>(TempRow + 1, TempCol);
                    int already = SegmentationImg.At<byte>(TempRow + 1, TempCol);
                    if (Math.Abs(pix - currPix) <= ThreshHold && already == 255)
                    {
                        PixelList.Add(new Pixel(TempRow + 1, TempCol));
                        SegmentationImg.Set<byte>(TempRow + 1, TempCol, label);
                    }
                }
                //----------------------------------------left
                if (isInRange(TempRow, TempCol - 1))
                {
                    int pix = src.At<byte>(TempRow, TempCol - 1);
                    int already = SegmentationImg.At<byte>(TempRow, TempCol - 1);
                    if (Math.Abs(pix - currPix) <= ThreshHold && already == 255)
                    {
                        PixelList.Add(new Pixel(TempRow, TempCol - 1));
                        SegmentationImg.Set<byte>(TempRow, TempCol - 1, label);
                    }
                }
                //----------------------------------------right
                if (isInRange(TempRow, TempCol + 1))
                {
                    int pix = src.At<byte>(TempRow, TempCol + 1);
                    int already = SegmentationImg.At<byte>(TempRow, TempCol + 1);
                    if (Math.Abs(pix - currPix) <= ThreshHold && already == 255)
                    {
                        PixelList.Add(new Pixel(TempRow, TempCol + 1));
                        SegmentationImg.Set<byte>(TempRow, TempCol + 1, label);
                    }
                }



            }//----------------------------end of while

            globalsegment = SegmentationImg;
            PixelList.Clear();
            using (new Window("Segmented Image", SegmentationImg))
            {
                Cv2.WaitKey(time);

            }
            Console.WriteLine("Done");

            return SegmentationImg;
            //Console.ReadKey();
            
        }
    }
}

