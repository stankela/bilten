﻿using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface StartListaNaSpraviDAO : GenericDAO<StartListaNaSpravi, int>
    {
        IList<StartListaNaSpravi> FindByGimnasticar(GimnasticarUcesnik g);
    }
}