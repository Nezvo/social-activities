import React from 'react';
import { Segment, Form, Button } from 'semantic-ui-react';

interface IProps {
  setEditMode: (editMode: boolean) => void;
}

const ActivityForm: React.FC<IProps> = ({ setEditMode }) => {
  return (
    <Segment clearing>
      <Form>
        <Form.Input placeholder="title" />
        <Form.TextArea rows="2" placeholder="description" />
        <Form.Input placeholder="category" />
        <Form.Input type="date" placeholder="date" />
        <Form.Input placeholder="city" />
        <Form.Input placeholder="venue" />
        <Button floated="right" positive type="submit" content="submit" />
        <Button
          onClick={() => setEditMode(false)}
          floated="right"
          type="submit"
          content="Cancel"
        />
      </Form>
    </Segment>
  );
};

export default ActivityForm;
