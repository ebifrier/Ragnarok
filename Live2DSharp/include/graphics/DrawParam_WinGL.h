/**
 * DrawParam_iPhone.h
 *
 *  Copyright(c) Live2D Inc. All rights reserved.
 *  [[ CONFIDENTIAL ]]
 */
#ifndef __LIVE2D_DRAWPARAM_WINGL_H__
#define __LIVE2D_DRAWPARAM_WINGL_H__

#include "../Live2D.h"

#ifdef L2D_TARGET_WIN_GL

#include "DrawParam.h"
#include "../type/LDVector.h"
#include <windows.h>
#include <GL/gl.h>


//------------ LIVE2D NAMESPACE ------------
namespace live2d
{ 
	

	/********************************************************************
	@brief		WindowsのOpenGL用描画クラス
	********************************************************************/
	class DrawParam_WinGL : public DrawParam 
	{
	public:
		// Constructor
		DrawParam_WinGL();
	
		// Destructor
		virtual ~DrawParam_WinGL();
		
	public:
		//  テクスチャ描画
		virtual void drawTexture( int textureNo  , int indexCount , int vertexCount , l2d_index * indexArray 
								 , l2d_pointf * vertexArray , l2d_uvmapf * uvArray , float opacity, int colorCompositionType) ;
	
		//  OpenGL用初期化
		static void initGLFunc() ;
	
		virtual void setupDraw();
	    
		//  テクスチャセット
		void setTexture( int modelTextureNo , GLuint textureNo ) ;
	
		
		
		// 新しく利用できるModelのテクスチャ番号を確保(Avatar用）
		virtual int generateModelTextureNo() ;
		
		// Modelのテクスチャ番号を生成(Avatar用）
		virtual void releaseModelTextureNo(int no) ;
		
	
		//  エラーチェック
		static int ___checkError___( const char* str ) ;
		static bool 				initGLFuncSuccess ;

	private:
		static const int DUMMY_GL_TEXTURE_NO = 9999 ;// generateした番号に立てるフラグ
		
	private:
		DrawParam_WinGL( const DrawParam_WinGL & );
		DrawParam_WinGL& operator=( const DrawParam_WinGL & );
	
	private:
		//  
	 	static void* my_wglGetProcAddress( const char* name ) ;
	
	
		//  FBO初期化
	    static void initFBO() ;

		//  FBO解放
	    static void disposeFBO() ;
	
		//  シェーダ初期化
	    static void initShader() ;
	    
		//  シェーダ解放
	    static void disposeShader() ;
	
		//  シェーダコンパイル
	    static bool compileShader(GLuint *shader , GLenum type , const char *shaderSource ) ;
	
		//  リンク
	    static bool linkProgram(GLuint prog) ;
	
		//  
	    static bool validateProgram(GLuint prog) ;
	
		//  シェーダロード
	    static bool loadShaders2() ;
	    
	private:
		//--------- fields ------------
		live2d::LDVector<GLuint> 	textures ;					// Destructorでテクスチャの解放は行わない。基本的に外部で行う。
		
	    
	    //--------- 2.0 ----------
	    static GLuint 				shaderProgram ;				//  シェーダ(OpenGL2.0)
		static bool 				first_initGLFunc ;

		
	
	};
}
//------------ LIVE2D NAMESPACE ------------
#endif		// L2D_TARGET_WIN_GL
#endif		// __LIVE2D_DRAWPARAM_WINGL_H__
