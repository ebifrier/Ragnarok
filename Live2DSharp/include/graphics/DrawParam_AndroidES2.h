/**
 *  DrawParam_iPhone.h
 *
 *  Copyright(c) Live2D Inc. All rights reserved.
 *  [[ CONFIDENTIAL ]]
 */
#ifndef __LIVE2D_DRAWPARAM_ANDROIDES2_H__
#define __LIVE2D_DRAWPARAM_ANDROIDES2_H__


#include "../Live2D.h"

#ifdef L2D_TARGET_ANDROID_ES2

#include <jni.h>
#include <errno.h>

//#include <EGL/egl.h>
#include <GLES2/gl2.h>

#include "../type/LDVector.h"
#include "DrawParam.h"

//------------ LIVE2D NAMESPACE ------------
namespace live2d
{ 
	
	/****************************************************************************
	@brief	AndroidES2専用の描画クラス
	****************************************************************************/
	class DrawParam_AndroidES2 : public DrawParam 
	{
	public:
		// Constructor
		DrawParam_AndroidES2();

		// Destructor
		virtual ~DrawParam_AndroidES2();
		
	public:
		//  テクスチャ描画
		virtual void drawTexture( int textureNo  , int indexCount , int vertexCount , l2d_index * indexArray 
								 , l2d_pointf * vertexArray , l2d_uvmapf * uvArray , float opacity, int colorCompositionType) ;
	
		static void setExtShaderMode( bool extMdoe , bool extPAMode = false ) ;
		
		//  DrawParamにテクスチャを設定する
		void setTexture( int modelTextureNo , GLuint textureNo ) ;
	    
		
		//  新しく利用できるModelのテクスチャ番号を確保(Avatar用）
		virtual int generateModelTextureNo() ;
		
		//  Modelのテクスチャ番号を生成(Avatar用）
		virtual void releaseModelTextureNo(int no) ;
		
		virtual void setupDraw();
		static GLuint fboTextureNo;
		static void setFramebufferTexture( GLuint fbo );
		static GLuint getFramebufferTexture();
		
	private:
		static const int DUMMY_GL_TEXTURE_NO = 9999 ;		// generateした番号に立てるフラグ
		
	private:
		//Prevention of copy Constructor
		DrawParam_AndroidES2( const DrawParam_AndroidES2 & ) ;				
		DrawParam_AndroidES2& operator=( const DrawParam_AndroidES2 & ) ; 	
		
	private:
		//
	    static void initFBO() ;
		//
	    static void disposeFBO() ;

		//  シェーダ初期化
	    static void initShader() ;
	    
		//  シェーダ解放
	    static void disposeShader() ;
	
		//  シェーダコンパイル
	    static bool compileShader(GLuint *shader , GLenum type , const char *shaderSource ) ;
	
		//  リンクする
	    static bool linkProgram(GLuint prog) ;
	
		//  
	    static bool validateProgram(GLuint prog) ;
	
		//  シェーダロード
  	  static GLuint loadShaders(const char* vertShaderSrc, const char * fragShaderSrc ) ;
	    
	    
	private:
		//--------- fields ------------
		live2d::LDVector<GLuint>* 		texturesPtr ;			//  Destructorでテクスチャの解放は行わない。基本的に外部で行う。
	    
	    //--------- ES 2.0 ----------
	    static GLuint 					shaderProgram ;
	    
		static bool			extMode ;					// 拡張方式で描画
		static bool			extPAMode ;					// 拡張方式で描画
	};
	
}
//------------ LIVE2D NAMESPACE ------------

#endif		// L2D_TARGET_ANDROID_ES2


#endif		// __LIVE2D_DRAWPARAM_ANDROIDES2_H__
