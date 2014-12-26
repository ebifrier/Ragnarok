#pragma once

#include "CppSharp.h"
#include <io/data/Arrays2D.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class Arrays2D;
}

namespace Live2DSharp
{
    /// <summary>
    /// 多次元配列を戻り値にするためのクラス
    /// </summary>
    public ref class Arrays2D : Live2DSharp::LDObject
    {
    public:

        Arrays2D(::live2d::Arrays2D* native);
        static Arrays2D^ __CreateInstance(::System::IntPtr native);
        Arrays2D(void** ptr, int size1, int size2);
    };
}
