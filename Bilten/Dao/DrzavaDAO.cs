﻿using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface DrzavaDAO : GenericDAO<Drzava, int>
    {
        IList<Drzava> FindAll();
        Drzava FindByKod(string kod);
        bool existsDrzavaNaziv(string naziv);
        bool existsDrzavaKod(string kod);
    }
}
