/*
 * TypeExtensions.cs
 *
 *   Created: 2022-12-22-11:24:11
 *   Modified: 2022-12-22-11:24:11
 *
 *   Author: Justin Chase <justin@justinwritescode.com>
 *
 *   Copyright © 2022-2023 Justin Chase, All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace PhoneNumbers;
using System.Reflection;
using System;

public static class TypeExtensions
{
    public static Assembly GetAssembly(this Type type)
    {
        return type.GetTypeInfo().Assembly;
    }
}
