

class InputBox extends React.Component {

	render() {
		return (
			<form>
				<label>
					Name:
					<input type="text" value={1} />
				</label>
				<input type="submit" value="Submit" />
			</form>
		);
	}

}

ReactDOM.render(
	<InputBox />,
	document.getElementById('input')
);