using UnityEngine;

namespace Rikhil.AnimationSystem
{
  public enum LoopType
{
    Restart,    // goes back to start each loop
    Yoyo,       // forward then backward
    Incremental // adds offset each loop
}

}
