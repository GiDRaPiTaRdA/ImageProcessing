//**********************************************
// Project: Flood Fill Algorithms in C# & GDI+
// File Description: Flood Fill Class
//
// Copyright: Copyright 2003 by Justin Dunlap.
//    Any code herein can be used freely in your own 
//    applications, provided that:
//     * You agree that I am NOT to be held liable for
//       any damages caused by this code or its use.
//     * You give proper credit to me if you re-publish
//       this code.
//**********************************************

using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageProcessing.MagicWand.Magic {
    /// <summary>
	/// TODO - Add class summary
	/// </summary>
	/// <remarks>
	/// 	created by - J Dunlap
	/// 	created on - 7/2/2003 11:44:33 PM
	/// </remarks>
	public class FloodFiller : AbstractFloodFiller {
		
		private int m_fillcolor=255<<8;
		
		/// <summary>
		/// Default constructor - initializes all fields to default values
		/// </summary>
		public FloodFiller() {
		}
		
		
		///<summary>initializes the FloodFill operation</summary>
		public override void FloodFill(Bitmap bmp, Point pt)
		{
			int ctr=timeGetTime();
			
			//Debug.WriteLine("*******Flood Fill******");
			
			//get the color's int value, and convert it from RGBA to BGRA format (as GDI+ uses BGRA)
                        this.m_fillcolor=ColorTranslator.ToWin32(this.m_fillcolorcolor);
			this.m_fillcolor=BGRA(GetB(this.m_fillcolor),GetG(this.m_fillcolor),GetR(this.m_fillcolor),GetA(this.m_fillcolor));
			
			//get the bits
                        BitmapData bmpData=bmp.LockBits(new Rectangle(0,0,bmp.Width,bmp.Height),ImageLockMode.ReadWrite,PixelFormat.Format32bppArgb);
			System.IntPtr Scan0 = bmpData.Scan0;
		    
		    unsafe
		    {
		    	//resolve pointer
		        byte * scan0=(byte *)(void *)Scan0;
		    	//get the starting color
		    	//[loc += Y offset + X offset]
		    	int loc=this.CoordsToIndex(pt.X,pt.Y,bmpData.Stride);//((bmpData.Stride*(pt.Y-1))+(pt.X*4));
		    	int color= *((int*)(scan0+loc));
		    	
		    	//create the array of bools that indicates whether each pixel
                        //has been checked.  (Should be bitfield, but C# doesn't support bitfields.)
                        this.PixelsChecked=new bool[bmpData.Width+1,bmpData.Height+1];
		    	
		    	//do the first call to the loop
		    	switch(this.m_FillStyle)
		    	{
		    		case FloodFillStyle.Linear :
				    	if(this.m_FillDiagonal)
				    	{
				    		this.LinearFloodFill8(scan0,pt.X,pt.Y,new Size(bmpData.Width,bmpData.Height),bmpData.Stride,(byte*)&color);
				    	}else{
				    		this.LinearFloodFill4(scan0,pt.X,pt.Y,new Size(bmpData.Width,bmpData.Height),bmpData.Stride,(byte*)&color);
				    	}
				    	break;
		    		case FloodFillStyle.Queue :
		    			this.QueueFloodFill(scan0,pt.X,pt.Y,new Size(bmpData.Width,bmpData.Height),bmpData.Stride,(byte*)&color);
		    			break;
		    		case FloodFillStyle.Recursive :
				    	if(this.m_FillDiagonal)
				    	{
				    		this.RecursiveFloodFill8(scan0,pt.X,pt.Y,new Size(bmpData.Width,bmpData.Height),bmpData.Stride,(byte*)&color);
				    	}else{
				    		this.RecursiveFloodFill4(scan0,pt.X,pt.Y,new Size(bmpData.Width,bmpData.Height),bmpData.Stride,(byte*)&color);
				    	}
				    	break;
		        }
		    }
		    
		    bmp.UnlockBits(bmpData);
			
                    this.m_TimeBenchmark=timeGetTime()-ctr;
			
		}
		
		//***********
		//LINEAR ALGORITHM
		//***********
		
		unsafe void LinearFloodFill4( byte* scan0, int x, int y,Size bmpsize, int stride, byte* startcolor)
		{
			
			//offset the pointer to the point passed in
			int* p=(int*) (scan0+(this.CoordsToIndex(x,y, stride)));
			
			
			//FIND LEFT EDGE OF COLOR AREA
			int LFillLoc=x; //the location to check/fill on the left
			int* ptr=p; //the pointer to the current location
			while(true)
			{
				ptr[0]=this.m_fillcolor; 	 //fill with the color
				this.PixelsChecked[LFillLoc,y]=true;
				LFillLoc--; 		 	 //de-increment counter
				ptr-=1;				 	 //de-increment pointer
				if(LFillLoc<=0 || !this.CheckPixel((byte*)ptr,startcolor) ||  (this.PixelsChecked[LFillLoc,y]))
					break;			 	 //exit loop if we're at edge of bitmap or color area
				
			}
			LFillLoc++;
			
			//FIND RIGHT EDGE OF COLOR AREA
			int RFillLoc=x; //the location to check/fill on the left
			ptr=p;
			while(true)
			{
				ptr[0]=this.m_fillcolor; //fill with the color
				this.PixelsChecked[RFillLoc,y]=true;
				RFillLoc++; 		 //increment counter
				ptr+=1;				 //increment pointer
				if(RFillLoc>=bmpsize.Width || !this.CheckPixel((byte*)ptr,startcolor) ||  (this.PixelsChecked[RFillLoc,y]))
					break;			 //exit loop if we're at edge of bitmap or color area
				
			}
			RFillLoc--;
			
			
			//START THE LOOP UPWARDS AND DOWNWARDS			
			ptr=(int*)(scan0+this.CoordsToIndex(LFillLoc,y,stride));
			for(int i=LFillLoc;i<=RFillLoc;i++)
			{
				//START LOOP UPWARDS
				//if we're not above the top of the bitmap and the pixel above this one is within the color tolerance
				if(y>0 && this.CheckPixel((byte*)(scan0+this.CoordsToIndex(i,y-1,stride)),startcolor) && (!(this.PixelsChecked[i,y-1])))
					this.LinearFloodFill4(scan0, i,y-1,bmpsize,stride,startcolor);
				//START LOOP DOWNWARDS
				if(y<(bmpsize.Height-1) && this.CheckPixel((byte*)(scan0+this.CoordsToIndex(i,y+1,stride)),startcolor) && (!(this.PixelsChecked[i,y+1])))
					this.LinearFloodFill4(scan0, i,y+1,bmpsize,stride,startcolor);
				ptr+=1;
			}
			
		}
		
		unsafe void LinearFloodFill8( byte* scan0, int x, int y,Size bmpsize, int stride, byte* startcolor)
		{
			
			//offset the pointer to the point passed in
			int* p=(int*) (scan0+(this.CoordsToIndex(x,y, stride)));
			
			
			//FIND LEFT EDGE OF COLOR AREA
			int LFillLoc=x; //the location to check/fill on the left
			int* ptr=p; //the pointer to the current location
			while(true)
			{
				ptr[0]=this.m_fillcolor; 	 //fill with the color
				this.PixelsChecked[LFillLoc,y]=true;
				LFillLoc--; 		 	 //de-increment counter
				ptr-=1;				 	 //de-increment pointer
				if(LFillLoc<=0 || !this.CheckPixel((byte*)ptr,startcolor) ||  (this.PixelsChecked[LFillLoc,y]))
					break;			 	 //exit loop if we're at edge of bitmap or color area
				
			}
			LFillLoc++;
			
			//FIND RIGHT EDGE OF COLOR AREA
			int RFillLoc=x; //the location to check/fill on the left
			ptr=p;
			while(true)
			{
				ptr[0]=this.m_fillcolor; //fill with the color
				this.PixelsChecked[RFillLoc,y]=true;
				RFillLoc++; 		 //increment counter
				ptr+=1;				 //increment pointer
				if(RFillLoc>=bmpsize.Width || !this.CheckPixel((byte*)ptr,startcolor) ||  (this.PixelsChecked[RFillLoc,y]))
					break;			 //exit loop if we're at edge of bitmap or color area
				
			}
			RFillLoc--;
			
			
			//START THE LOOP UPWARDS AND DOWNWARDS			
			ptr=(int*)(scan0+this.CoordsToIndex(LFillLoc,y,stride));
			for(int i=LFillLoc;i<=RFillLoc;i++)
			{
				//START LOOP UPWARDS
				//if we're not above the top of the bitmap and the pixel above this one is within the color tolerance
				//START LOOP DOWNWARDS
				if(y>0)
				{
					//UP
					if(this.CheckPixel((byte*)(scan0+this.CoordsToIndex(i,y-1,stride)),startcolor) && (!(this.PixelsChecked[i,y-1])))
						this.LinearFloodFill8(scan0, i,y-1,bmpsize,stride,startcolor);
					//UP-LEFT
					if(x>0 && this.CheckPixel((byte*)(scan0+this.CoordsToIndex(i-1,y-1,stride)),startcolor) && (!(this.PixelsChecked[i-1,y-1])))
						this.LinearFloodFill8(scan0, i-1,y-1,bmpsize,stride,startcolor);
					//UP-RIGHT
					if(x<(bmpsize.Width-1) && this.CheckPixel((byte*)(scan0+this.CoordsToIndex(i+1,y-1,stride)),startcolor) && (!(this.PixelsChecked[i+1,y-1])))
						this.LinearFloodFill8(scan0, i+1,y-1,bmpsize,stride,startcolor);
				}
				
				if(y<(bmpsize.Height-1)) 
				{
					//DOWN
					if(this.CheckPixel((byte*)(scan0+this.CoordsToIndex(i,y+1,stride)),startcolor) && (!(this.PixelsChecked[i,y+1])))
						this.LinearFloodFill8(scan0, i,y+1,bmpsize,stride,startcolor);
					//DOWN-LEFT
					if(x>0 && this.CheckPixel((byte*)(scan0+this.CoordsToIndex(i-1,y+1,stride)),startcolor) && (!(this.PixelsChecked[i-1,y+1])))
						this.LinearFloodFill8(scan0, i-1,y+1,bmpsize,stride,startcolor);
					//UP-RIGHT
					if(x<(bmpsize.Width-1) && this.CheckPixel((byte*)(scan0+this.CoordsToIndex(i+1,y+1,stride)),startcolor) && (!(this.PixelsChecked[i+1,y+1])))
						this.LinearFloodFill8(scan0, i+1,y+1,bmpsize,stride,startcolor);
					
				}
				
				ptr+=1;
			}
			
		}
		
		//*********
		//RECURSIVE ALGORITHM
		//*********
		
    	public unsafe void RecursiveFloodFill8(byte* scan0, int x, int y,Size bmpsize, int stride, byte* startcolor)
    	{
			//don't go over the edge
        	if ((x < 0) || (x >= bmpsize.Width)) return;
        	if ((y < 0) || (y >= bmpsize.Height)) return;
    		int* p=(int*) (scan0+(this.CoordsToIndex(x,y, stride)));
        	if (!(this.PixelsChecked[x,y]) && this.CheckPixel((byte*) p,startcolor)) 
        	{
				p[0]=this.m_fillcolor; 	 //fill with the color
				this.PixelsChecked[x,y]=true;
        		
        		//branch in all 8 directions
            	this.RecursiveFloodFill8(scan0,x+1, y,bmpsize,stride, startcolor);
            	this.RecursiveFloodFill8(scan0,x, y+1,bmpsize,stride, startcolor);
            	this.RecursiveFloodFill8(scan0,x-1, y,bmpsize,stride, startcolor);
            	this.RecursiveFloodFill8(scan0,x, y-1,bmpsize,stride, startcolor);
            	this.RecursiveFloodFill8(scan0,x+1, y+1,bmpsize,stride, startcolor);
            	this.RecursiveFloodFill8(scan0,x-1, y+1,bmpsize,stride, startcolor);
            	this.RecursiveFloodFill8(scan0,x-1, y-1,bmpsize,stride, startcolor);
            	this.RecursiveFloodFill8(scan0,x+1, y-1,bmpsize,stride, startcolor);
        	}
    	}

    	public unsafe void RecursiveFloodFill4(byte* scan0, int x, int y,Size bmpsize, int stride, byte* startcolor)
    	{
			//don't go over the edge
        	if ((x < 0) || (x >= bmpsize.Width)) return;
        	if ((y < 0) || (y >= bmpsize.Height)) return;
    		
    		//calculate pointer offset
    		int* p=(int*) (scan0+(this.CoordsToIndex(x,y, stride)));
    		
    		//if the pixel is within the color tolerance, fill it and branch out
        	if (!(this.PixelsChecked[x,y]) && this.CheckPixel((byte*) p,startcolor)) 
        	{
				p[0]=this.m_fillcolor; 	 //fill with the color
				this.PixelsChecked[x,y]=true;
        		
            	this.RecursiveFloodFill4(scan0,x+1, y,bmpsize,stride, startcolor);
            	this.RecursiveFloodFill4(scan0,x, y+1,bmpsize,stride, startcolor);
            	this.RecursiveFloodFill4(scan0,x-1, y,bmpsize,stride, startcolor);
            	this.RecursiveFloodFill4(scan0,x, y-1,bmpsize,stride, startcolor);
        	}
    	}
		
		//********
		//QUEUE ALGORITHM
		//********
		
		public unsafe void QueueFloodFill(byte* scan0, int x, int y,Size bmpsize, int stride, byte* startcolor)
		{
			this.CheckQueue=new Queue();
			
			if(!this.m_FillDiagonal)
			{
				//start the loop
				this.QueueFloodFill4(scan0,x,y,bmpsize,stride,startcolor);
				//call next item on queue
				while(this.CheckQueue.Count>0)
				{
					Point pt=(Point)this.CheckQueue.Dequeue();
					this.QueueFloodFill4(scan0,pt.X,pt.Y,bmpsize,stride,startcolor);
				}
			}else{
				//start the loop
				this.QueueFloodFill8(scan0,x,y,bmpsize,stride,startcolor);
				//call next item on queue
				while(this.CheckQueue.Count>0)
				{
					Point pt=(Point)this.CheckQueue.Dequeue();
					this.QueueFloodFill8(scan0,pt.X,pt.Y,bmpsize,stride,startcolor);
				}
			}
		}
		
    	public unsafe void QueueFloodFill8(byte* scan0, int x, int y,Size bmpsize, int stride, byte* startcolor)
    	{
			//don't go over the edge
        	if ((x < 0) || (x >= bmpsize.Width)) return;
        	if ((y < 0) || (y >= bmpsize.Height)) return;
    		int* p=(int*) (scan0+(this.CoordsToIndex(x,y, stride)));
        	if (!(this.PixelsChecked[x,y]) && this.CheckPixel((byte*) p,startcolor)) 
        	{
				p[0]=this.m_fillcolor; 	 //fill with the color
				this.PixelsChecked[x,y]=true;
        		
                        this.CheckQueue.Enqueue(new Point(x+1,y));
        		this.CheckQueue.Enqueue(new Point(x,y+1));
        		this.CheckQueue.Enqueue(new Point(x-1,y));
        		this.CheckQueue.Enqueue(new Point(x,y-1));
				
                        this.CheckQueue.Enqueue(new Point(x+1,y+1));
        		this.CheckQueue.Enqueue(new Point(x-1,y-1));
        		this.CheckQueue.Enqueue(new Point(x-1,y+1));
        		this.CheckQueue.Enqueue(new Point(x+1,y-1));
        	}
    	}

    	public unsafe void QueueFloodFill4(byte* scan0, int x, int y,Size bmpsize, int stride, byte* startcolor)
    	{
			//don't go over the edge
        	if ((x < 0) || (x >= bmpsize.Width)) return;
        	if ((y < 0) || (y >= bmpsize.Height)) return;
    		
    		//calculate pointer offset
    		int* p=(int*) (scan0+(this.CoordsToIndex(x,y, stride)));
    		
    		//if the pixel is within the color tolerance, fill it and branch out
        	if (!(this.PixelsChecked[x,y]) && this.CheckPixel((byte*) p,startcolor)) 
        	{
				p[0]=this.m_fillcolor; 	 //fill with the color
				this.PixelsChecked[x,y]=true;
        		
				this.CheckQueue.Enqueue(new Point(x+1,y));
        		this.CheckQueue.Enqueue(new Point(x,y+1));
        		this.CheckQueue.Enqueue(new Point(x-1,y));
        		this.CheckQueue.Enqueue(new Point(x,y-1));
        	}
    	}		
		
		//*********
		//HELPER FUNCTIONS
		//*********
		
		///<summary>Sees if a pixel is within the color tolerance range.</summary>
		//px - a pointer to the pixel to check
		//startcolor - a pointer to the color of the pixel we started at
		unsafe bool CheckPixel(byte* px, byte* startcolor)
		{
                        bool ret=true;
			for(byte i=0;i<3;i++)
				ret&= (px[i]>= (startcolor[i]-this.m_Tolerance[i])) && px[i] <= (startcolor[i]+this.m_Tolerance[i]);		    
		    return ret;
		}
		
		///<summary>Given X and Y coordinates and the bitmap's stride, returns a pointer offset</summary>
		int CoordsToIndex(int x, int y, int stride)
		{
			return (stride*y)+(x*4);
		}
		
		
		
	}


}
