using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITimeSlotChangeEventListener
{
   public void TimeSlotChangeEventHandler(EnumTimeSlot timeSlot);

   public void AssignEventHandler();
}
