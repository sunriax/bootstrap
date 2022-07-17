using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace BootstrapLibTest
{
    public enum ErrorCode
    {
        OK,
        ERROR,
        TEST
    }

    public class DemoConfig
    {
        private IEnumerable<DemoArrayConfig> array;
        private int value;

        public int Value
        {
            get => this.value;
            set
            {
                if (value < 1)
                    throw new ArgumentException(nameof(DemoConfig));
                this.value = value;
            }
        }

        public IEnumerable<DemoArrayConfig> Array
        {
            get => this.array;
            set
            {
                value.ToList().ForEach(e =>
                {
                    if (e.Error != ErrorCode.OK)
                        throw new ArgumentException(nameof(DemoArrayConfig));
                });

                this.array = value;
            }
        }
    }

    public class DemoArrayConfig
    {
        private int value;

        public int Value
        {
            get => this.value;
            set
            {
                this.Error = ErrorCode.OK;

                if (value < 1)
                    this.Error = ErrorCode.ERROR;

                this.value = value;
            }
        }

        public ErrorCode Error { get; private set; }
    }
}

