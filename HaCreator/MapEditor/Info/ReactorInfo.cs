/* Copyright (C) 2015 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using HaCreator.GUI;
using HaCreator.MapEditor.Instance;
using HaCreator.Wz;
using HaSharedLibrary.Wz;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using System.Collections.Generic;
using System.Drawing;

namespace HaCreator.MapEditor.Info
{
    public class ReactorInfo : MapleExtractableInfo
    {
        private readonly string id;
        private readonly string cat;
        private readonly string art;
        private readonly string _name;

        private WzSubProperty _LinkedWzObj;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="image"></param>
        /// <param name="origin"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="parentObject"></param>
        public ReactorInfo(Bitmap image, System.Drawing.Point origin, string id, string name, WzObject parentObject, string cat, string art)
            : base(image, origin, parentObject)
        {
            this.id = id;
            this._name = name;
            this.cat = cat;
            this.art = art;
        }

        private void ExtractPNGFromImage(WzSubProperty obj)
        {
            WzCanvasProperty reactorImage = WzInfoTools.GetReactorImage(obj);
            if (reactorImage != null)
            {
                Image = reactorImage.GetLinkedWzCanvasBitmap();
                Origin = WzInfoTools.PointFToSystemPoint(reactorImage.GetCanvasOriginPosition());
            }
            else
            {
                Image = new Bitmap(1, 1);
                Origin = new System.Drawing.Point();
            }
        }

        public override void ParseImage()
        {
            ExtractPNGFromImage(LinkedWzObj);
        }

        public static ReactorInfo Get(string id)
        {
            ReactorInfo result = Program.InfoManager.Reactors[id];
            result.ParseImageIfNeeded();
            return result;
        }

        public override BoardItem CreateInstance(Layer layer, Board board, int x, int y, int z, bool flip)
        {
            if (Image == null) 
                ParseImage();
            return new ReactorInstance(this, board, layer, x, y, z, UserSettings.defaultReactorTime, "", flip);
        }

        public string ID
        {
            get { return id; }
            private set { }
        }

        public string Name
        {
            get { return _name; }
            private set { }
        }

        /// <summary>
        /// The source WzImage of the reactor
        /// </summary>
        public WzSubProperty LinkedWzObj
        {
            get {
                if (_LinkedWzObj == null) {
                    var obj = Program.WzManager.FindWzImageByName("map", "Obj");
                    _LinkedWzObj = (WzSubProperty)WzInfoTools.GetObjectByRelativePath(obj, $"Reactor.img/{cat}/{art}/{id}");
                }
                return _LinkedWzObj;
            }
            set { this._LinkedWzObj = value; }
        }
    }
}
