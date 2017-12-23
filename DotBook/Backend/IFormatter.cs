﻿using DotBook.Model;
using DotBook.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Backend
{
    public interface IFormatter<TOut, TIn>
        where TIn : INameable, IDocumentable, INode<INameable>
    {
        TOut Process(TIn entity);
    }
}