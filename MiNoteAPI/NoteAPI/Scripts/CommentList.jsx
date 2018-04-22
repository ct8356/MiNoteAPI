import Comment from 'Comment';

var CommentList = React.createClass({

  render: function() {
    return (
      <div className="commentList">
        <Comment author="Daniel Lo Nigro">Hello ReactJS.NET World!</Comment>
        <Comment author="Pete Hunt">This is one comment</Comment>
        <Comment author="Jordan Walke">This is *another* comment</Comment>
      </div>
	);
  }

});

export default CommentList;