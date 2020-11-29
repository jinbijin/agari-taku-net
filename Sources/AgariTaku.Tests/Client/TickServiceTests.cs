using AgariTaku.Client.Services;
using AgariTaku.Shared.Common;
using AgariTaku.Shared.Types;
using AgariTaku.Tests.Common;
using FluentAssertions;
using FluentAssertions.Execution;
using System;
using System.Linq;
using Xunit;

namespace AgariTaku.Tests.Client
{
    public class TickServiceTests
    {
        private readonly ClientState _clientState;
        private readonly TickService _tickService;

        public TickServiceTests()
        {
            IConfiguration configuration = new TestConfiguration();
            _clientState = new ClientState(configuration);
            _tickService = new TickService(configuration, _clientState);
        }

        [Fact]
        public void AddTickShouldIncrementCurrentTick()
        {
            // Act
            _tickService.AddTick();

            // Assert
            _clientState.CurrentTick.Should().Be(0);
        }

        [Fact]
        public void AddTickShouldAddClientTickToBuffer()
        {
            // Act
            _tickService.AddTick();

            // Assert
            _clientState.ClientTickBuffer.Slice(0, 5).Select(tick => tick?.TickNumber).Should().Equal(0, null, null, null, null);
        }

        [Fact]
        public void AddTickShouldNotAckTicks()
        {
            // Act
            _tickService.AddTick();

            // Assert
            _clientState.AckTicks.ToArray().Should().Equal(-1, -1, -1, -1, -1);
        }

        [Fact]
        public void AddTickShouldNotAddServerTickToBuffer()
        {
            // Act
            _tickService.AddTick();

            // Assert
            using AssertionScope scope = new();
            foreach (TickSource source in (TickSource[]) Enum.GetValues(typeof(TickSource)))
            {
                _clientState.ServerTickBuffer.Slice(source, 0, 5).Select(tick => tick?.TickNumber).Should().Equal(null, null, null, null, null);
            }
        }

        [Fact]
        public void AddTickMultipleShouldIncrementCurrentTick()
        {
            // Act
            for (int i = 0; i < 13; i++)
            {
                _tickService.AddTick();
            }

            // Assert
            _clientState.CurrentTick.Should().Be(12);
        }

        [Fact]
        public void AddTickMultipleShouldAddClientTicksCircularly()
        {
            // Act
            for (int i = 0; i < 13; i++)
            {
                _tickService.AddTick();
            }

            // Assert
            _clientState.ClientTickBuffer.Slice(0, 5).Select(tick => tick?.TickNumber).Should().Equal(10, 11, 12, 8, 9);
        }
    }
}
