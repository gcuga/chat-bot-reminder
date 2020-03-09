using System;
using Xunit;
using Reminder.Storage.InMemory;
using Reminder.Storage.Entity;
using System.Collections.Generic;

namespace Reminder.Storage.InMemory.Tests
{
	public class StorageInMemoryBaseThroughUsersInMemoryTests
	{
		[Fact]
		public void Method_Insert_With_Not_Null_Item_Should_Store_The_Item_Internally()
		{
			// prepare test data
			string userName = "Test Method_Insert_With_Not_Null_Item_Should_Store_The_Item_Internally";
			var storage = UsersInMemory.Instance;
			var expected = new User(storage, userName);

			// do the test
			storage.Insert(expected);

			// check the results
			var actual = storage.Get(expected.Id);
			Assert.NotNull(actual);
			Assert.Equal(expected.Id, actual.Id);
			Assert.Equal(expected.Name, actual.Name);
			Assert.Equal(expected.Name, actual.Name);
			Assert.Equal(userName, actual.Name);
		}

		[Fact]
		public void Method_Get_By_Id_Should_Raise_KeyNotFoundException_For_Empty_Storage()
		{
			// prepare test data
			var storage = UsersInMemory.Instance;
			storage._storage.Clear();

			// act & assert
			Assert.Throws<KeyNotFoundException>(() => { storage.Get(0); });
		}

		[Fact]
		public void Method_Get_By_Id_Should_Return_Not_Null_For_Existing_Item_In_Storage()
		{
			// prepare test data
			string userName = "Test  Method_Get_By_Id_Should_Return_Not_Null_For_Existing_Item_In_Storage";
			var storage = UsersInMemory.Instance;
			var expected = new User(storage, userName);
			storage._storage.Add(expected.Id, expected);

			// do the test
			var actual = storage.Get(expected.Id);

			// check the results
			Assert.NotNull(actual);
		}

		[Fact]
		public void Method_TryGet_By_Id_Should_Return_False_And_Null_As_Out_For_Empty_Storage()
		{
			// prepare test data
			var storage = UsersInMemory.Instance;
			storage._storage.Clear();

			// act & assert
			Assert.False(storage.TryGet(0, out User actual));
			Assert.Null(actual);
		}

		[Fact]
		public void Method_TryGet_By_Id_Should_Return_True_And_Not_Null_As_Out_For_Existing_Item_In_Storage()
		{
			// prepare test data
			string userName = "Test Method_TryGet_By_Id_Should_Return_True_And_Not_Null_As_Out_For_Existing_Item_In_Storage";
			var storage = UsersInMemory.Instance;
			var expected = new User(storage, userName);
			storage._storage.Add(expected.Id, expected);

			// act & assert
			Assert.True(storage.TryGet(expected.Id, out User actual));
			Assert.NotNull(actual);
		}
	}
}
