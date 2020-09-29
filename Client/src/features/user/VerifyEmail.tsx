import React, { useContext, useEffect, useState } from 'react';
import { RouteComponentProps } from 'react-router-dom';
import { RootStoreContext } from '../../app/stores/rootStore';
import queryString from 'query-string';
import agent from '../../app/api/agent';
import { Button, Header, Icon, Segment } from 'semantic-ui-react';
import LoginForm from './LoginForm';
import { toast } from 'react-toastify';

const VerifyEmail: React.FC<RouteComponentProps> = ({ location }) => {
	const rootStore = useContext(RootStoreContext);
	const Status = {
		Verifying: 'Verifying',
		Failed: 'Failed',
		Success: 'Success',
	};
	const [status, setstatus] = useState(Status.Verifying);
	const { openModal } = rootStore.modalStore;
	const { token, email } = queryString.parse(location.search);

	useEffect(() => {
		agent.User.verifyEmail(token as string, email as string)
			.then(() => setstatus(Status.Success))
			.catch(() => setstatus(Status.Success));
	}, [agent, Status, token, email]);

	const handleResendEmailVerification = () => {
		agent.User.resendEmailVerification(email as string)
			.then(() =>
				toast.success('Verification email resent - please check your email')
			)
			.catch((error) => console.log(error));
	};

	const getBody = () => {
		switch (status) {
			case Status.Verifying:
				return <p>Verifying...</p>;
			case Status.Failed:
				return (
					<div className="center">
						<p>
							Verification failed - you can try resending the verification email
						</p>
						<Button
							primary
							onClick={handleResendEmailVerification}
							size="huge"
							content="Resend email"
						/>
					</div>
				);
			case Status.Success:
				return (
					<div className="center">
						<p>Email has beed verified - you can now login</p>
						<Button
							primary
							onClick={() => openModal(<LoginForm />)}
							size="large"
							content="Login"
						/>
					</div>
				);
		}
	};
	return (
		<Segment placeholder>
			<Header icon>
				<Icon name="envelope" />
				Email verification
			</Header>
			<Segment.Inline>{getBody()}</Segment.Inline>
		</Segment>
	);
};

export default VerifyEmail;
