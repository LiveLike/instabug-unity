extern "C" {
    void logToiOS(const char* debugMessage) {
        NSLog(@"%@", [NSString stringWithUTF8String:debugMessage]);
    }
}
