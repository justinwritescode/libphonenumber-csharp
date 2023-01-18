/*
 * Copyright (C) 2011 Patrick Mezard
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PhoneNumbers.Test, PublicKey=0024000004800000940000000602000000240000525341310004000001000100fdd39d9591f6d6230e2afd153a1a235c30383ff9be61743b4b3d1cdab2a00fce5e12ee9ca0ab184f0827ba956f47db9d95f30921a70e250cc39bda7770f2f9e57294ef74336acb6d09670dc26f912157585e81c6766f41e8569290c62a2264e361c897f1e4a957e60eaa923d39d666319ffc990e0bec2f11a7d46b165d32069c")]

namespace PhoneNumbers
{
    internal class EnumerableFromConstructor<T> : IEnumerable<T>
    {
        private readonly Func<IEnumerator<T>> fn;

        public EnumerableFromConstructor(Func<IEnumerator<T>> fn)
        {
            this.fn = fn;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return fn();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return fn();
        }
    }
}
