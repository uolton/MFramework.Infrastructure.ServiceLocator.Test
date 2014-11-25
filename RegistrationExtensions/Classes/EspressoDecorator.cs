/*   Copyright 2009 - 2010 Marcus Bratton

     Licensed under the Apache License, Version 2.0 (the "License");
     you may not use this file except in compliance with the License.
     You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

     Unless required by applicable law or agreed to in writing, software
     distributed under the License is distributed on an "AS IS" BASIS,
     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     See the License for the specific language governing permissions and
     limitations under the License.
*/

namespace MFramework.Infrastructure.ServiceLocator.Test.RegistrationExtensions.Classes
{
    public class EspressoShotDecorator : IngredientDecorator
    {
        public EspressoShotDecorator(ICoffee decoratedCoffee) : base(decoratedCoffee)
        {
        }

        public override string Name
        {
            get { return string.Format("{0}, shot of espresso", base.Name); }
        }

        public override decimal Total
        {
            get { return base.Total + 0.50m; }
        }
    }
}
