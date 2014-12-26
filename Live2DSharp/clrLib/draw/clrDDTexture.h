#pragma once

#include "CppSharp.h"
#include <draw/DDTexture.h>
#include "draw/clrIDrawContext.h"
#include "draw/clrIDrawData.h"

namespace Live2DSharp
{
    ref class BReader;
    ref class DDTexture;
    ref class DDTextureContext;
    ref class DrawParam;
    ref class MemoryParam;
    ref class ModelContext;
}

namespace Live2DSharp
{
    public ref class DDTexture : Live2DSharp::IDrawData
    {
    public:

        DDTexture(::live2d::DDTexture* native);
        static DDTexture^ __CreateInstance(::System::IntPtr native);
        DDTexture();

        property int TextureNo
        {
            int get();
            void set(int);
        }

        property float* UvMap
        {
            float* get();
        }

        property int NumPoints
        {
            int get();
        }

        property int NumPolygons
        {
            int get();
        }

        property int Type
        {
            int get();
        }

        property int OptionFlag
        {
            int get();
        }

        property int Option_KanojoColor
        {
            int get();
        }

        virtual void readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam) override;

        void initDirect(Live2DSharp::MemoryParam^ memParam);

        virtual Live2DSharp::IDrawContext^ init(Live2DSharp::ModelContext^ mdc) override;

        virtual void setupInterpolate(Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ cdata) override;

        virtual void setupTransform(Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ cdata) override;

        virtual void draw(Live2DSharp::DrawParam^ dp, Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ cdata) override;

        virtual void setZ_TestImpl(Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ _cdata, float z) override;

        unsigned short* getIndexArray(int* polygonCount);

        static property int OPTION_FLAG_BARCODE_KANOJO_COLOR_CONVERT
        {
            int get();
        }

        static property int MASK_COLOR_COMPOSITION
        {
            int get();
        }

        static property int COLOR_COMPOSITION_NORMAL
        {
            int get();
        }

        static property int COLOR_COMPOSITION_SCREEN
        {
            int get();
        }

        static property int COLOR_COMPOSITION_MULTIPLY
        {
            int get();
        }

        static property int INSTANCE_COUNT
        {
            int get();
            void set(int);
        }
    };

    public ref class DDTextureContext : Live2DSharp::IDrawContext
    {
    public:

        DDTextureContext(::live2d::DDTextureContext* native);
        static DDTextureContext^ __CreateInstance(::System::IntPtr native);
        DDTextureContext(Live2DSharp::IDrawData^ src);

        property int tmpBaseDataIndex
        {
            int get();
            void set(int);
        }

        property float* interpolatedPoints
        {
            float* get();
            void set(float*);
        }

        property float* transformedPoints
        {
            float* get();
            void set(float*);
        }

        property float* drawPoints
        {
            float* get();
            void set(float*);
        }

        property unsigned char not_updated_count
        {
            unsigned char get();
            void set(unsigned char);
        }

        float* getTransformedPoints(int* pointCount);
    };
}
