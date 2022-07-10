using System;
using System.Collections.Generic;
using System.Text;

namespace ServerModel.Logic.Component
{
    public interface IComponent
    {
        GameObject GameObject { get; set; }

        public void OnInit();

        public void OnRelease();
    }
}
