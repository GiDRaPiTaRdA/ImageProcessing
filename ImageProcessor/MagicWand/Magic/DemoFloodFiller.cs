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

using System;
using System.Collections;
using System.Drawing;

namespace ImageProcessing.MagicWand.Magic {
    /// <summary>
	/// TODO - Add class summary
	/// </summary>
	/// <remarks>
	/// 	created by - J Dunlap
	/// 	created on - 7/2/2003 11:44:33 PM
	/// </remarks>
	public class DemoFloodFiller : AbstractFloodFiller {
		
		protected Color m_fillcolor=Color.Green;
		int m_Interval=20;
		int times=0;
		
		
		/// <summary>
		/// Default constructor - initializes all fields to default values
		/// </summary>
		public DemoFloodFiller() {
		}
		
		
		public int Interval
		{
			get
			{
				return this.m_Interval;
			}
			set
			{
				this.m_Interval=value;
			}
		}
		
		
		//initializes the FloodFill operation
		public override void FloodFill(Bitmap bmp, Point pt)
		{
			int ctr=timeGetTime();
			this.times=0;
			
			//Debug.WriteLine("*******Flood Fill******");
			
			this.m_fillcolor=this.m_fillcolorcolor;

		    	//get the starting color
		    	//[loc += Y offset + X offset]
		    	Color color= bmp.GetPixel(pt.X,pt.Y);
		    	
		    	this.PixelsChecked=new bool[bmp.Width+1,bmp.Height+1];
		    	
		    	//do the first call to the loop
		    	switch(this.m_FillStyle)
		    	{
		    		case FloodFillStyle.Linear :
				    	if(this.m_FillDiagonal)
				    	{
				    		this.LinearFloodFill8(bmp,pt.X,pt.Y,color,0);
				    	}else{
				    		this.LinearFloodFill4(bmp,pt.X,pt.Y,color,0);
				    	}
				    	break;
		    		case FloodFillStyle.Queue :
		    			this.QueueFloodFill(bmp,pt.X,pt.Y,color,0);
		    			break;
		    		case FloodFillStyle.Recursive :
				    	if(this.m_FillDiagonal)
				    	{
				    		this.RecursiveFloodFill8(bmp,pt.X,pt.Y,color,0);
				    	}else{
				    		this.RecursiveFloodFill4(bmp,pt.X,pt.Y,color,0);
				    	}
				    	break;
		    	}
			
			this.m_TimeBenchmark=timeGetTime()-ctr;
			
		}
		
		//***********
		//LINEAR ALGORITHM
		//***********
		
		//this overload of FloodFill does the "grunt work"
		unsafe void LinearFloodFill4(Bitmap bmp, int x, int y, Color startcolor,int lvl)
		{
			
			
			//FIND LEFT EDGE OF COLOR AREA
			int LFillLoc=x; //the location to check/fill on the left
			while(true)
			{
				bmp.SetPixel(LFillLoc,y,this.m_fillcolor); 	 //fill with the color
				this.PixelsChecked[LFillLoc,y]=true;
				this.OnFillProgress(new FillProgressEventArgs(new Point(LFillLoc,y),lvl,this.times));
				LFillLoc--; 		 	 //de-increment counter
				if(LFillLoc<=0 || !this.CheckPixel(bmp.GetPixel(LFillLoc,y),startcolor) ||  (this.PixelsChecked[LFillLoc,y]))
					break;			 	 //exit loop if we're at edge of bitmap or color area
				
			}
			LFillLoc++;
			
			//FIND RIGHT EDGE OF COLOR AREA
			int RFillLoc=x; //the location to check/fill on the left
			while(true)
			{
				bmp.SetPixel(RFillLoc,y,this.m_fillcolor); 	 //fill with the color
				this.PixelsChecked[RFillLoc,y]=true;
				this.OnFillProgress(new FillProgressEventArgs(new Point(RFillLoc,y),lvl,this.times));
				RFillLoc++; 		 //increment counter
				if(RFillLoc>=bmp.Width || !this.CheckPixel(bmp.GetPixel(RFillLoc,y),startcolor) ||  (this.PixelsChecked[RFillLoc,y]))
					break;			 //exit loop if we're at edge of bitmap or color area
				
			}
			RFillLoc--;
			
			
			//START THE LOOP UPWARDS AND DOWNWARDS			
			for(int i=LFillLoc;i<=RFillLoc;i++)
			{
				//START LOOP UPWARDS
				//if we're not above the top of the bitmap and the pixel above this one is within the color tolerance
				if(y>0 && this.CheckPixel(bmp.GetPixel(i,y-1),startcolor) && (!(this.PixelsChecked[i,y-1])))
					this.LinearFloodFill4(bmp, i,y-1,startcolor,lvl+1);
				//START LOOP DOWNWARDS
				if(y<(bmp.Height-1) && this.CheckPixel(bmp.GetPixel(i,y+1),startcolor) && (!(this.PixelsChecked[i,y+1])))
					this.LinearFloodFill4(bmp, i,y+1,startcolor,lvl+1);
			}
			
		}
		
		//this overload of FloodFill does the "grunt work"
		void LinearFloodFill8(Bitmap bmp, int x, int y, Color startcolor, int lvl)
		{
			
			
			//FIND LEFT EDGE OF COLOR AREA
			int LFillLoc=x; //the location to check/fill on the left
			while(true)
			{
				bmp.SetPixel(LFillLoc,y,this.m_fillcolor); 	 //fill with the color
				this.PixelsChecked[LFillLoc,y]=true;
				this.OnFillProgress(new FillProgressEventArgs(new Point(LFillLoc,y),lvl,this.times));
				LFillLoc--; 		 	 //de-increment counter
				if(LFillLoc<=0 || !this.CheckPixel(bmp.GetPixel(LFillLoc,y),startcolor) ||  (this.PixelsChecked[LFillLoc,y]))
					break;			 	 //exit loop if we're at edge of bitmap or color area
				
			}
			LFillLoc++;
			
			//FIND RIGHT EDGE OF COLOR AREA
			int RFillLoc=x; //the location to check/fill on the left
			while(true)
			{
				bmp.SetPixel(RFillLoc,y,this.m_fillcolor); //fill with the color
				this.OnFillProgress(new FillProgressEventArgs(new Point(RFillLoc,y),lvl,this.times));
				this.PixelsChecked[RFillLoc,y]=true;
				RFillLoc++; 		 //increment counter
				if(RFillLoc>=bmp.Width || !this.CheckPixel(bmp.GetPixel(RFillLoc,y),startcolor) ||  (this.PixelsChecked[RFillLoc,y]))
					break;			 //exit loop if we're at edge of bitmap or color area
				
			}
			RFillLoc--;
			
			
			//START THE LOOP UPWARDS AND DOWNWARDS			
			for(int i=LFillLoc;i<=RFillLoc;i++)
			{
				//START LOOP UPWARDS
				//if we're not above the top of the bitmap and the pixel above this one is within the color tolerance
				//START LOOP DOWNWARDS
				if(y>0)
				{
					//UP
					if(this.CheckPixel(bmp.GetPixel(i,y-1),startcolor) && (!(this.PixelsChecked[i,y-1])))
						this.LinearFloodFill8(bmp, i,y-1,startcolor,lvl+1);
					//UP-LEFT
					if(x>0 && this.CheckPixel(bmp.GetPixel(i-1,y-1),startcolor) && (!(this.PixelsChecked[i-1,y-1])))
						this.LinearFloodFill8(bmp, i-1,y-1,startcolor,lvl+1);
					//UP-RIGHT
					if(x<(bmp.Width-1) && this.CheckPixel(bmp.GetPixel(i+1,y-1),startcolor) && (!(this.PixelsChecked[i+1,y-1])))
						this.LinearFloodFill8(bmp, i+1,y-1,startcolor,lvl+1);
				}
				
				if(y<(bmp.Height-1)) 
				{
					//DOWN
					if(this.CheckPixel(bmp.GetPixel(i,y+1),startcolor) && (!(this.PixelsChecked[i,y+1])))
						this.LinearFloodFill8(bmp, i,y+1,startcolor,lvl+1);
					//DOWN-LEFT
					if(x>0 && this.CheckPixel(bmp.GetPixel(i-1,y+1),startcolor) && (!(this.PixelsChecked[i-1,y+1])))
						this.LinearFloodFill8(bmp, i-1,y+1,startcolor,lvl+1);
					//UP-RIGHT
					if(x<(bmp.Width-1) && this.CheckPixel(bmp.GetPixel(i+1,y+1),startcolor) && (!(this.PixelsChecked[i+1,y+1])))
						this.LinearFloodFill8(bmp, i+1,y+1,startcolor,lvl+1);
					
				}

			}
			
		}
		
		//*********
		//RECURSIVE ALGORITHM
		//*********
		
    	public unsafe void RecursiveFloodFill8(Bitmap bmp, int x, int y, Color startcolor, int lvl)
    	{
			//don't go over the edge
        	if ((x < 0) || (x >= bmp.Width)) return;
        	if ((y < 0) || (y >= bmp.Height)) return;
    		
        	if (!(this.PixelsChecked[x,y]) && this.CheckPixel(bmp.GetPixel(x,y),startcolor)) 
        	{
				bmp.SetPixel(x,y,this.m_fillcolor); 	 //fill with the color
				this.PixelsChecked[x,y]=true;
        		this.OnFillProgress(new FillProgressEventArgs(new Point(x,y),lvl,this.times));
        		
            	this.RecursiveFloodFill8(bmp,x+1, y, startcolor,lvl+1);
            	this.RecursiveFloodFill8(bmp,x, y+1, startcolor,lvl+1);
            	this.RecursiveFloodFill8(bmp,x-1, y, startcolor,lvl+1);
            	this.RecursiveFloodFill8(bmp,x, y-1, startcolor,lvl+1);
            	this.RecursiveFloodFill8(bmp,x+1, y+1, startcolor,lvl+1);
            	this.RecursiveFloodFill8(bmp,x-1, y+1, startcolor,lvl+1);
            	this.RecursiveFloodFill8(bmp,x-1, y-1, startcolor,lvl+1);
            	this.RecursiveFloodFill8(bmp,x+1, y-1, startcolor,lvl+1);
        	}
    	}

		//:)
    	public void RecursiveFloodFill4(Bitmap bmp, int x, int y, Color startcolor, int lvl)
    	{
			//don't go over the edge
        	if ((x < 0) || (x >= bmp.Width)) return;
        	if ((y < 0) || (y >= bmp.Height)) return;
    		
    		//calculate pointer offset
    		
    		//if the pixel is within the color tolerance, fill it and branch out
        	if (!(this.PixelsChecked[x,y]) && this.CheckPixel(bmp.GetPixel(x,y),startcolor)) 
        	{
				bmp.SetPixel(x,y,this.m_fillcolor); 	 //fill with the color
				this.PixelsChecked[x,y]=true;
        		this.OnFillProgress(new FillProgressEventArgs(new Point(x,y),lvl,this.times));
        		
            	this.RecursiveFloodFill4(bmp,x+1, y, startcolor,lvl+1);
            	this.RecursiveFloodFill4(bmp,x, y+1, startcolor,lvl+1);
            	this.RecursiveFloodFill4(bmp,x-1, y, startcolor,lvl+1);
            	this.RecursiveFloodFill4(bmp,x, y-1, startcolor,lvl+1);
        	}
    	}
		
		//********
		//QUEUE ALGORITHM
		//********
		
		//:)
		public void QueueFloodFill(Bitmap bmp, int x, int y, Color startcolor, int lvl)
		{
			this.CheckQueue=new Queue();
			
			if(!this.m_FillDiagonal)
			{
				//start the loop
				this.QueueFloodFill4(bmp,x,y,startcolor,0);
				//call next item on queue
				while(this.CheckQueue.Count>0)
				{
					Point pt=(Point)this.CheckQueue.Dequeue();
					this.QueueFloodFill4(bmp,pt.X,pt.Y,startcolor,0);
				}
			}else{
				//start the loop
				this.QueueFloodFill8(bmp,x,y,startcolor,0);
				//call next item on queue
				while(this.CheckQueue.Count>0)
				{
					Point pt=(Point)this.CheckQueue.Dequeue();
					this.QueueFloodFill8(bmp,pt.X,pt.Y,startcolor,0);
				}
			}
		}
		
		//:)
    	public void QueueFloodFill8(Bitmap bmp, int x, int y, Color startcolor, int lvl)
    	{
			//don't go over the edge
        	if ((x < 0) || (x >= bmp.Width)) return;
        	if ((y < 0) || (y >= bmp.Height)) return;
    		
        	if (!(this.PixelsChecked[x,y]) && this.CheckPixel(bmp.GetPixel(x,y),startcolor)) 
        	{
				bmp.SetPixel(x,y,this.m_fillcolor); 	 //fill with the color
				this.PixelsChecked[x,y]=true;
        		this.OnFillProgress(new FillProgressEventArgs(new Point(x,y),lvl,this.times));
        		
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

		//:)
    	public void QueueFloodFill4(Bitmap bmp, int x, int y, Color startcolor, int lvl)
    	{
			//don't go over the edge
        	if ((x < 0) || (x >= bmp.Width)) return;
        	if ((y < 0) || (y >= bmp.Height)) return;
    		
    		//calculate pointer offset
    		
    		//if the pixel is within the color tolerance, fill it and branch out
        	if (!(this.PixelsChecked[x,y]) && this.CheckPixel(bmp.GetPixel(x,y),startcolor)) 
        	{
				bmp.SetPixel(x,y,this.m_fillcolor); 	 //fill with the color
				this.PixelsChecked[x,y]=true;
        		this.OnFillProgress(new FillProgressEventArgs(new Point(x,y),lvl,this.times));
        		
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
		bool CheckPixel(Color px, Color startcolor)
		{
			return px.R>= (startcolor.R-this.m_Tolerance[0]) && px.R <= (startcolor.R+this.m_Tolerance[0]) &&
				px.G>= (startcolor.G-this.m_Tolerance[1]) && px.G <= (startcolor.G+this.m_Tolerance[1]) &&
				px.B>= (startcolor.B-this.m_Tolerance[2]) && px.B <= (startcolor.B+this.m_Tolerance[2]);
		}
	
		
		public event FillProgressEventHandler FillProgress;
		void OnFillProgress(FillProgressEventArgs args)
		{
			if(this.FillProgress!=null)
				//FillProgress.BeginInvoke(this,args,null,null);
				this.FillProgress(this,args);
		}		
		
	}

	public delegate void FillProgressEventHandler(object sender, FillProgressEventArgs e);
	public class FillProgressEventArgs : EventArgs
	{
		Point m_CurrentPt;
		int m_StackLevel;
		int m_TimesCalled;
		
		
		public FillProgressEventArgs(Point pt, int stacklevel, int timescalled)
		{
			this.m_StackLevel=stacklevel;
			this.m_TimesCalled=timescalled;
			this.m_CurrentPt=pt;
		}
		
		public Point CurrentPt
		{
			get
			{
				return this.m_CurrentPt;
			}
		}
		
		public int StackLevel
		{
			get
			{
				return this.m_StackLevel;
			}
		}
		
		public int TimesCalled
		{
			get
			{
				return this.m_TimesCalled;
			}
		}
	}

}
