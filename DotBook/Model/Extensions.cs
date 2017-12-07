﻿using DotBook.Model.Entities;
using DotBook.Model.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotBook.Model
{
    public static class Extensions
    {
        public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> source,
            IComparer<T> comparer = null) => new SortedSet<T>(source, comparer);

        public static void AddOrReuse<T1, T2>(this T2 info, T1 decl, SortedSet<T2> set)
            where T2 : IPartial<T1>
        {
            if (set == null) return;
            set.Add(info);
            var savedInfo = set.Where(c => c.Equals(info)).First();
            savedInfo.Populate(decl);
        }

        public static void AddAsChild(this INameable parent, 
            MemberDeclarationSyntax s,
            SortedSet<ClassInfo> classes = null,
            SortedSet<StructInfo> structs = null,
            SortedSet<InterfaceInfo> interfaces = null,
            SortedSet<EnumInfo> enums = null,
            SortedSet<FieldInfo> fields = null,
            SortedSet<PropertyInfo> properties = null,
            SortedSet<IndexerInfo> indexers = null,
            SortedSet<MethodInfo> methods = null)
        {
            // ugly but works
            switch (s)
            {
                case ClassDeclarationSyntax decl:
                    new ClassInfo(decl, parent).AddOrReuse(decl, classes); break;
                case EnumDeclarationSyntax decl:
                    enums?.Add(new EnumInfo(decl, parent)); break;
                case StructDeclarationSyntax decl:
                    new StructInfo(decl, parent).AddOrReuse(decl, structs); break;
                case InterfaceDeclarationSyntax decl:
                    new InterfaceInfo(decl, parent).AddOrReuse(decl, interfaces); break;
                case FieldDeclarationSyntax decl:
                    fields?.Add(new FieldInfo(decl, parent)); break;
                case PropertyDeclarationSyntax decl:
                    properties?.Add(new PropertyInfo(decl, parent)); break;
                case IndexerDeclarationSyntax decl:
                    indexers?.Add(new IndexerInfo(decl, parent)); break;
                case MethodDeclarationSyntax decl:
                    methods?.Add(new MethodInfo(decl, parent)); break;
            }
        }

        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
    }
}
