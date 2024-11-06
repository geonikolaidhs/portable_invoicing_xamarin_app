using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;

namespace ITS.WRM.SFA.Model.Enumerations
{
    [Flags]
    public enum eMachineStatus
    {
        [WrmDisplay(Name = "UNKNOWN_MACHINE_STATUS", ResourceType = typeof(ResourcesRest))]
        UNKNOWN = 1,                    //Άγνωστη κατάσταση μηχανής
        [WrmDisplay(Name = "SALE", ResourceType = typeof(ResourcesRest))]
        SALE=2,                       //Η μηχανή έχει κάνει εναρξη ημέρας, κ βάρδιας, αλλά δεν έχει ανοιχτο παραστατικό
        [WrmDisplay(Name = "OPENDOCUMENT", ResourceType = typeof(ResourcesRest))]
        OPENDOCUMENT=4,               //Η μηχανή έχει κάνει εναρξη ημέρας, κ βάρδιας, αλλά έχει ανοιχτο παραστατικό κ χτυπάει είδη
        [WrmDisplay(Name = "OPENDOCUMENT_PAYMENT", ResourceType = typeof(ResourcesRest))]
        OPENDOCUMENT_PAYMENT=8,       //Η μηχανή έχει κάνει εναρξη ημέρας, κ βάρδιας, αλλά έχει ανοιχτο παραστατικό κ έχει πατήσει σύνολο
        [WrmDisplay(Name = "CLOSED_MACHINE_STATUS", ResourceType = typeof(ResourcesRest))]
        CLOSED=16,                     //Η μηχανή ΔΕΝ έχει κάνει εναρξη ημέρας
        [WrmDisplay(Name = "PAUSE", ResourceType = typeof(ResourcesRest))]
        PAUSE=32,                      //Η μηχανή ειναι κλειδωμένη
        [WrmDisplay(Name = "DAYSTARTED", ResourceType = typeof(ResourcesRest))]
        DAYSTARTED=64,                 //Η μηχανή έχει κάνει εναρξη ημέρας, αλλά όχι βάρδιας
       [WrmDisplay(Name = "OPENDRAWER", ResourceType = typeof(ResourcesRest))]
        OPENDRAWER =128                  //Tο συρτάρι είναι ανοιχτό
    }
}
