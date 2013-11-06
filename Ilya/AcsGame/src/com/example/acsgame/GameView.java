package com.example.acsgame;

import android.annotation.SuppressLint;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Canvas;
import android.graphics.Color;
import android.view.MotionEvent;
import android.view.SurfaceHolder;
import android.view.SurfaceView;
import android.view.View;
import android.view.View.OnTouchListener;
import android.widget.TextView;
 @SuppressLint("WrongCall")
public class GameView extends SurfaceView implements OnTouchListener  {  
    /**��������� ������*/
    private Bitmap bmp;
    TextView tv;
    float x;
    float y;
    /**���� ���������*/
    private SurfaceHolder holder;
    
    /**������ ������ GameView*/
    private GameManager gameLoopThread;
    
    /**������ ������ Sprite*/
    private Sprite sprite;
   
    /**�����������*/
    public GameView(Context context) 
    {
    	  
          super(context);
          gameLoopThread = new GameManager(this);
          holder = getHolder();
          
          /*������ ��� ���� ������� � ��� ��� ���*/
          holder.addCallback(new SurfaceHolder.Callback() 
          {
                        /*** ����������� ������� ��������� */
                 public void surfaceDestroyed(SurfaceHolder holder) 
                 {
                        boolean retry = true;
                        gameLoopThread.setRunning(false);
                        while (retry) 
                        {
                               try 
                               {
                                     gameLoopThread.join();
                                     retry = false;
                               } catch (InterruptedException e) 
                               {
                               }
                        }
                 }

                 /** �������� ������� ��������� */
                 public void surfaceCreated(SurfaceHolder holder) 
                 {
                        gameLoopThread.setRunning(true);
                        gameLoopThread.start();
                 }

                 /** ��������� ������� ��������� */
                 public void surfaceChanged(SurfaceHolder holder, int format,
                               int width, int height) 
                 {
                 }
          });
          bmp = BitmapFactory.decodeResource(getResources(), R.drawable.image);
          sprite = new Sprite(this,bmp);
    }

    /**������� �������� ��� ������� � ���
     * @return */
   
    protected void onDraw(Canvas canvas) 
    {
          canvas.drawColor(Color.BLACK);
          sprite.onDraw(canvas);
    }

	@Override
	public boolean onTouch(View v, MotionEvent event) {
		
	      return false;
		// TODO Auto-generated method stub
	}

	
   
}