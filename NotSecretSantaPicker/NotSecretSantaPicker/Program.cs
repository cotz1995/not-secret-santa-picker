namespace NotSecretSantaPicker
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var people = new List<Person> { 
				new Person("Michael", "Holley"),
				new Person("Holley", "Michael"),
				new Person("Jim", "Pam"),
				new Person("Pam", "Jim"),
				new Person("Phillis"),
				new Person("Kevin"),
				new Person("Dwight"),
				new Person("Angela"),
				new Person("Oscar"),
				new Person("Toby"),
				new Person("Kelly"),
				new Person("Ryan"),
				new Person("Stanley"),
				new Person("Meredith"),
				new Person("Creed"),
			};

			var picker = new SantaPicker();

			Console.WriteLine("Picking Santas...");

			var santas = picker.PickSantas(people);

			if(santas == null || santas.Count == 0) 
			{
				Console.WriteLine("Something went wrong...no assignments made.");
				return;
			}

			Console.WriteLine("Here are the Santa assignments! May the odds be ever in your favor!");
			foreach (var santa in santas)
			{
				Console.WriteLine($"{santa.SantaName} is Santa for {santa.RecipientName}");
			}
		}
	}

	public class Person
	{
		public string Name { get; set; }
		public string? SignificantOther { get; set; }

		public Person(string name, string? significantOther = null)
		{
			Name = name;
			SignificantOther = significantOther;
		}
	}

	public class SantaAssignment
	{
		public string SantaName { get; set; }
		public string RecipientName { get; set; }

		public SantaAssignment(string santaName, string recipientName)
		{
			SantaName = santaName;
			RecipientName = recipientName;
		}
	}

	public class SantaPicker
	{
		public List<SantaAssignment>? PickSantas(List<Person> people)
		{
			var successfulResult = false;

			List<SantaAssignment>? santaAssignments = null;
			while (!successfulResult)
			{
				santaAssignments = new List<SantaAssignment>();
				var remainingPeople = people.Select(x => x).ToList();
				for (int i = 0; i < people.Count; i++)
				{
					var santa = PickSanta(people, remainingPeople, santaAssignments, i);
					// invalid result - bail
					if (santa == null)
						break;

					santaAssignments.Add(santa);
				}

				if(santaAssignments.Count == people.Count)
				{
					successfulResult = true;
				}

				var resultWord = successfulResult ? "successful" : "unsuccessful";
                Console.WriteLine($"Attempt was {resultWord}");
			}

			return santaAssignments;
		}

		private static SantaAssignment? PickSanta(List<Person> people, List<Person> remainingPeople, List<SantaAssignment> santaAssignments, int i)
		{
			var random = new Random(Guid.NewGuid().GetHashCode());

			do
			{
				var index = random.Next(remainingPeople.Count);

				// if the only remaining person is self, bail invalid solution
				if (remainingPeople.Count == 1 && people[i] == remainingPeople[index])
				{
					return null;
				}
				
				// if the only remaining people are not a valid assignment, bail
				if (remainingPeople.Count < 3 && !IsValidSantaAssignment(people[i], remainingPeople[index], santaAssignments))
				{
					return null;
				}

				// if the recipient and santa are different people and is a valid assignment, ship it
				if (people[i] != remainingPeople[index] && IsValidSantaAssignment(people[i], remainingPeople[index], santaAssignments))
				{
					var santa = people[i];
					var recipient = remainingPeople[index];

					remainingPeople.Remove(recipient);

					return new SantaAssignment(santaName:santa.Name, recipientName: recipient.Name);
				}
			} while (true);
		}

		private static bool IsValidSantaAssignment(Person potentialSanta, Person potentialRecipient, List<SantaAssignment> santaAssignments)
		{
			return !IsSignificantOther(potentialSanta, potentialRecipient) && !IsBidirectionalAssignment(potentialSanta, potentialRecipient, santaAssignments);
		}

		private static bool IsSignificantOther(Person personA, Person personB)
		{
			return personA.SignificantOther == personB.Name;
		}

		/// <summary>
		/// determins if the potential assignment will result in a bidirectional santa relationship
		/// </summary>
		/// <param name="potentialSanta"></param>
		/// <param name="potentialRecipient"></param>
		/// <param name="santaAssignments"></param>
		/// <returns></returns>
		private static bool IsBidirectionalAssignment(Person potentialSanta, Person potentialRecipient, List<SantaAssignment> santaAssignments)
		{
			return santaAssignments.Any(x => x.SantaName == potentialRecipient.Name && x.RecipientName == potentialSanta.Name);
		}
	}
}
