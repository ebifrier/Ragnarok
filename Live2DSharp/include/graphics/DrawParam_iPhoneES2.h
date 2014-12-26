/**
 *  DrawParam_iPhone.h
 *
 *  Copyright(c) Live2D Inc. All rights reserved.
 *  [[ CONFIDENTIAL ]]
 */
#ifndef __LIVE2D_DRAWPARAM_IPHONEES2_H__
#define __LIVE2D_DRAWPARAM_IPHONEES2_H__


#include "../Live2D.h"

#ifdef L2D_TARGET_IPHONE_ES2
#include <OpenGLES/ES2/gl.h>
#include <OpenGLES/ES2/glext.h>
#include "DrawParam.h"
#include "../type/LDVector.h"


//------------ LIVE2D NAMESPACE ------------
namespace live2d
{ 
	
	class DrawParam_iPhoneES2 : public DrawParam 
	{
	public:
		// Constructor
		DrawParam_iPhoneES2();
	
		// Destructor
		virtual ~DrawParam_iPhoneES2();
		
	public:
		//  描画
		virtual void drawTexture( int textureNo  , int indexCount , int vertexCount , l2d_index * indexArray 
								 , l2d_pointf * vertexArray , l2d_uvmapf * uvArray , float opacity, int colorCompositionType ) ;
	
	
		virtual void setupDraw();
	
	    
		//  DrawParamにテクスチャを設定
		void setTexture( int modelTextureNo , GLuint textureNo ) ;
	
		
		
		// 新しく利用できるModelのテクスチャ番号を確保(Avatar用）
		virtual int generateModelTextureNo() ;
		
		// Modelのテクスチャ番号を生成(Avatar用）
		virtual void releaseModelTextureNo(int no) ;
		
	private:
		static const int DUMMY_GL_TEXTURE_NO = 9999 ;// generateした番号に立てるフラグ
		
	private:
		//Prevention of copy Constructor
		DrawParam_iPhoneES2( const DrawParam_iPhoneES2 & ) ;				
		DrawParam_iPhoneES2& operator=( const DrawParam_iPhoneES2 & ) ; 	
	private:
		static void initShader() ;
	    
	    static void disposeShader() ;
	    static bool compileShader(GLuint *shader , GLenum type , const char *shaderSource ) ;
	    static bool linkProgram(GLuint prog) ;
	    static bool validateProgram(GLuint prog) ;
	    static bool loadShaders2() ;
	    
	private:
		//--------- fields ------------
		live2d::LDVector<GLuint> 		textures ;					// Destructorでテクスチャの解放は行わない。基本的に外部で行う。
	    
	    //--------- ES 2.0 ----------
	    static GLuint                   shaderProgram ;
	};
	
} 
//------------ LIVE2D NAMESPACE ------------

#endif //L2D_TARGET_IPHONE_ES2
#endif		// __LIVE2D_DRAWPARAM_IPHONEES2_H__
