﻿using System;

namespace MFramework.Infrastructure.ServiceLocator.Test.TestClasses
{
    public struct TestStruct
    {
        public int x, y;
        // Constructor:
        public TestStruct(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        // Override the ToString method:
        public override string ToString()
        {
            return (String.Format("({0},{1})", x, y));
        }
    }
}
