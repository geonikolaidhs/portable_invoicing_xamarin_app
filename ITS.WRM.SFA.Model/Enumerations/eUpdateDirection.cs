using System;

namespace ITS.WRM.SFA.Model.Enumerations
{
    [Flags]
    public enum eUpdateDirection
    {
        /*POS_CAN_SEND = 1,
        POS_CAN_RECEIVE = 2,
        STORECONTROLLER_CAN_SEND = 4,
        STORECONTROLLER_CAN_RECEIVE = 8,
        MASTER_CAN_SEND = 16,
        MASTER_CAN_RECEIVE = 32,*/
        NONE = 0,
        POS_TO_STORECONTROLLER = 1,
        STORECONTROLLER_TO_MASTER = 2,
        STORECONTROLLER_TO_POS = 4,
        MASTER_TO_STORECONTROLLER = 8,
        MASTER_TO_SFA = 16,
        SFA_TO_MASTER = 32

    }
}
