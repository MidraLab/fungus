﻿// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    public interface ISaveDataItemSerializer
    {
        int Order { get; }
        string DataTypeKey { get; }

        void PreDecode();

        void PostDecode();

        SaveDataItem[] Encode();

        bool Decode(SaveDataItem sdi);
    }
}