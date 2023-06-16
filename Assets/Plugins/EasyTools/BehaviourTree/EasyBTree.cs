using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools {

	public enum NodeState {
		Success,
		Failure,
	}

	public abstract class Node {
		public string name;

		public Node(string name) {
			this.name = name;
		}

		public abstract IEnumerator Run();

		public static bool WaitingForResult(IEnumerator run, out object current, out bool success) {
			success = false;
			current = null;
			if (run.MoveNext()) {
				current = run.Current;
				if (current is NodeState state) success = state == NodeState.Success;
				else return true;
			}
			return false;
		}
	}

	public abstract class NodeWithChild : Node, IEnumerable {
		public NodeWithChild(string name) : base(name) { }

		public abstract void Add(Node child);

		IEnumerator IEnumerable.GetEnumerator() => EnumChildren();
		public abstract IEnumerator EnumChildren();
	}

	public abstract class DecoratorNode : NodeWithChild {
		public Node child;

		public DecoratorNode(string name) : base(name) { }

		public override void Add(Node child) => this.child = child;

		public override IEnumerator EnumChildren() {
			yield return child;
		}
	}

	public abstract class NodeWithChildren : NodeWithChild {
		public List<Node> children = new();

		public NodeWithChildren(string name) : base(name) { }

		public override void Add(Node child) => children.Add(child);
		public override IEnumerator EnumChildren() => children.GetEnumerator();
	}


	public class Leaf : Node {
		public Leaf(string name) : base(name) { }

		public override IEnumerator Run() {
			yield return NodeState.Success;
		}
	}

	public class Sequence : NodeWithChildren {
		public Sequence(string name) : base(name) { }

		public override IEnumerator Run() {
			foreach (Node child in children) {
				var run = child.Run();
				var success = false;
				while (WaitingForResult(run, out var current, out success)) yield return current;
				if (!success) yield return NodeState.Failure;
			}
			yield return NodeState.Success;
		}
	}

	public class Selector : NodeWithChildren {
		public Selector(string name) : base(name) { }

		public override IEnumerator Run() {
			foreach (Node child in children) {
				var run = child.Run();
				var success = false;
				while (WaitingForResult(run, out var current, out success)) yield return current;
				if (success) yield return NodeState.Success;
			}
			yield return NodeState.Failure;
		}
	}

	public class Inverter : DecoratorNode {
		public Inverter(string name) : base(name) { }

		public override IEnumerator Run() {
			var run = child.Run();
			var success = false;
			while (WaitingForResult(run, out var current, out success)) yield return current;
			yield return success ? NodeState.Failure : NodeState.Success;
		}
	}

	public class UntilSuccess : DecoratorNode {
		public UntilSuccess(string name) : base(name) { }

		public override IEnumerator Run() {
			do {
				var run = child.Run();
				var success = false;
				while (WaitingForResult(run, out var current, out success)) yield return current;
				if (success) yield return NodeState.Success;
			} while (true);
		}
	}

	public class UntilFailure : DecoratorNode {
		public UntilFailure(string name) : base(name) { }

		public override IEnumerator Run() {
			do {
				var run = child.Run();
				var success = false;
				while (WaitingForResult(run, out var current, out success)) yield return current;
				if (!success) yield return NodeState.Failure;
			} while (true);
		}
	}


	public class BTree {
		public Node root = new Sequence("root") {
			new Selector("selector") {
				new Sequence("attack") {
					new UntilFailure("find target") {
						new Inverter("target dead") {
							new UntilSuccess("attack target") {
								new Leaf("attack"),
							},
						},
					},
					new Leaf("find another target"),
				},
				new Sequence("move") {
					new Leaf("find target"),
					new UntilSuccess("move to target") {
						new Leaf("move"),
					},
				},
			},
		};
	}
}
