#pragma once

#include "CppSharp.h"
#include <avatar/AvatarPartsItem.h>
#include "io/clrISerializableV2.h"

namespace Live2DSharp
{
    ref class AvatarPartsItem;
    ref class BReader;
    ref class IBaseData;
    ref class IDrawData;
    ref class MemoryParam;
    ref class PartsData;
    ref class PartsDataID;
}

namespace Live2DSharp
{
    public ref class AvatarPartsItem : Live2DSharp::ISerializableV2
    {
    public:

        AvatarPartsItem(::live2d::AvatarPartsItem* native);
        static AvatarPartsItem^ __CreateInstance(::System::IntPtr native);
        AvatarPartsItem();

        virtual void readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam) override;

        void replacePartsData(Live2DSharp::PartsData^ parts);

        static property int INSTANCE_COUNT
        {
            int get();
            void set(int);
        }
    };
}
