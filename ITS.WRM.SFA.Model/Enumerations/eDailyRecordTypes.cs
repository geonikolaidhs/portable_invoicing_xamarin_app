using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eDailyRecordTypes
    {
        DISCOUNTS,                  //0
        INVOICES,                   //1
        CANCELED_DOCUMENT_DETAIL,   //2
        CANCELED_DOCUMENT,          //3
        LOYALTYPOINTS,              //4
        DRAWERS,                    //5
        TAXRECORD,                  //6
        PAYMENTS,                   //7
        ITEMS,                      //8
        COUPONS,                    //9
        EMPTYBOTLLES,               //10
        RETURNS,                    //11
        USERS,                      //12
        CASH,                       //13
        DOCUMENT_DISCOUNTS,         //14
        CANCELED_RETURNS,           //15
        STARTING_AMOUNT,            //16
        CANCELLED_EDPS_PAYMENTS,    //17
        CASH_DIFFERENCE,            //18
        END_SHIFT_USER_COUNT_DOWN,   //19
        //for cashRegister
        ZERO_RECEIPTS               //20
    }
}
