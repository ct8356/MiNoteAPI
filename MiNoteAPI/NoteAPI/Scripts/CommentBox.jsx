import CommentForm from './CommentForm';

class CommentBox extends React.Component {

	render() {
		return (
			<div className="commentBox">
				<h1>Comments</h1>
				<CommentForm />
			</div>
		);
	}

}

ReactDOM.render(
	<CommentBox />,
	document.getElementById('content')
);