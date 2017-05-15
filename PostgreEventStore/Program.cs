using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using NpgsqlTypes;
using PostgreEventStore.Domain;
using PostgreEventStore.Utility;

namespace PostgreEventStore
{
    class Program
    {
        static void Main(string[] args)
        {
            using (
                var sqlConnection =
                    new NpgsqlConnection("Host=127.0.0.1;Username=postgres;Password=qwpo123;Database=EventStore"))
            {
                //// x2 events
                //var eventsToBeInserted = new List<Event>
                //{
                //        new Event
                //        {
                //            version = 0,
                //            data = ProtoSerializer.Serialize("event1"),
                //            date = DateTime.Now
                //        },
                //        new Event
                //        {
                //            version = 0,
                //            data = ProtoSerializer.Serialize("event2"),
                //            date = DateTime.Now
                //        }
                //};

                // x10 events
                var eventsToBeInserted = new List<Event>
                {
                    new Event()
                    {
                        version = 0,
                        data = ProtoSerializer.Serialize("event1"),
                        date = DateTime.Now
                    },
                    new Event()
                    {
                        version = 0,
                        data = ProtoSerializer.Serialize("event2"),
                        date = DateTime.Now
                    },
                    new Event()
                    {
                        version = 0,
                        data = ProtoSerializer.Serialize("event3"),
                        date = DateTime.Now
                    },
                    new Event()
                    {
                        version = 0,
                        data = ProtoSerializer.Serialize("event4"),
                        date = DateTime.Now
                    },
                    new Event()
                    {
                        version = 0,
                        data = ProtoSerializer.Serialize("event5"),
                        date = DateTime.Now
                    },
                    new Event()
                    {
                        version = 0,
                        data = ProtoSerializer.Serialize("event6"),
                        date = DateTime.Now
                    },
                    new Event()
                    {
                        version = 0,
                        data = ProtoSerializer.Serialize("event7"),
                        date = DateTime.Now
                    },
                    new Event()
                    {
                        version = 0,
                        data = ProtoSerializer.Serialize("event8"),
                        date = DateTime.Now
                    },
                    new Event()
                    {
                        version = 0,
                        data = ProtoSerializer.Serialize("event9"),
                        date = DateTime.Now
                    },
                    new Event()
                    {
                        version = 0,
                        data = ProtoSerializer.Serialize("event10"),
                        date = DateTime.Now
                    }
                };

                sqlConnection.Open();
                sqlConnection.MapComposite<Event>("eventtype");

                for (int i = 0; i < 3162; ++i)
                {
                    int expectedVersion = 1;
                    Guid aggregateId = Guid.NewGuid();

                    foreach (var e in eventsToBeInserted)
                        e.aggregateid = aggregateId.ToString();

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.CommandText = "saveevents";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = sqlConnection;

                        PrepareParameters(cmd, aggregateId, expectedVersion,
                            eventsToBeInserted);

                        ExecuteCommandInTrasaction(sqlConnection, cmd);                              
                    }
                }
            }

            Console.WriteLine("Work done.");
        }

        private static void ExecuteCommandInTrasaction(NpgsqlConnection sqlConnection, NpgsqlCommand command)
        {
            using (var transaction = sqlConnection.BeginTransaction())
            {
                try
                {
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                }
            }
        }

        private static void PrepareParameters(NpgsqlCommand command, Guid aggregateId, int expectedVersion, List<Event> events)
        {
            var parameter = new NpgsqlParameter("aggregatedidparam", NpgsqlDbType.Char)
            {
                Direction = ParameterDirection.Input,
                Value = aggregateId.ToString()
            };
            command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("expectedversion", NpgsqlDbType.Integer)
            {
                Direction = ParameterDirection.Input,
                Value = expectedVersion
            };
            command.Parameters.Add(parameter);

            parameter = new NpgsqlParameter("events", NpgsqlDbType.Array | NpgsqlDbType.Composite)
            {
                Direction = ParameterDirection.Input,
                Value = events.ToArray()
            };
            command.Parameters.Add(parameter);
        }
    }
}
