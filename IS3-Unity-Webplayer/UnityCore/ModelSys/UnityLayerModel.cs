using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace IS3.Unity.Webplayer.UnityCore
{
    public class UnityLayer
    {
        public string rootName = "iS3Project";
        public UnityTreeModel UnityLayerModel { get; set; } = new UnityTreeModel();
        public UnityLayer()
        {
           
        }
    }
    public class UnityTreeModel: INotifyPropertyChanged
    {
        public UnityTreeModel parent { get; set; }
        public string Name { get; set; }
        public int layer { get; set; }
        private bool _visible = true;
        public bool visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                if (PropertyChanged!=null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("visible"));
                }

            }
        }
        public List<UnityTreeModel> childs { get; set; }
        public UnityTreeModel()
        {
            childs = new List<UnityTreeModel>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    
}