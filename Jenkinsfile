pipeline {
    agent any
    tools {
        nodejs "nodejs"
        dockerTool "docker"
    }    
    stages {
        stage('Build') {
            steps {
                script {
                    // Remove previous image with the <none> tag
                    sh 'docker rmi acjobapi/api:latest || true'

                    // Build the new image with an explicit tag
                    sh 'docker build --no-cache -t acjobapi/api:latest -f Dockerfile .'

                    // Remove dangling images
                    sh 'docker images -q --filter "dangling=true" | xargs docker rmi || true'

                    // Clean up intermediate images
                    sh 'docker image prune -f'

                    // Custom cleanup script to remove any remaining <none> tagged images
                    sh '''
                        for image_id in $(docker images --filter "dangling=true" -q); do
                            docker rmi $image_id || true
                        done
                    '''
                }
            }
        }
        stage('Push and Deploy') {
            steps {                
                script {
                    // Stop and remove the container if it exists
                    sh 'docker stop accuracore_job || true'
                    sh 'docker rm accuracore_job || true'
                    
                    // Run the new container
                    sh 'docker run -d --name accuracore_job -p 2003:80 --env "ASPNETCORE_ENVIRONMENT=Staging" acjobapi/api:latest'
                }
            }
        }
    }
}
